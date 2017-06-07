using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using IVRPOC.Models;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

namespace IVRPOC
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public static List<Accountant_Information> accountnumlist;
        //public static StateClient stateClient = null;
        internal static IDialog<Customer> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(Customer.BuildForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        var completed = await order;
                        Random random = new Random();
                        int randomno = random.Next(1025518043, 2025518043);
                        string accno = randomno + "2";
                        Random rand = new Random();
                        int randno = rand.Next(0, 9);
                        int accpin = 1234+ randno;

                        await context.PostAsync("**These are the your Complete account details:** \r \n " + "**AcccountNumber: **" + accno + "\r \n " + "**Pin: **" + accpin +
                            "\r \n" + "**FullName: **" + completed.FullName + "\r \n " + "**AccountType: **" + completed.AccountType + "\r \n " + "**Personal Details: **" + completed.PersonalDetails + "\r \n"
                             + "**Correspondence Address: **" + completed.CorrespondenceAddress +
                            "\r \n " + "**Permanent Address: **" + completed.PermanentAddress + "\r \n " + "**Social Security Number: **" + completed.SocialSecurityNumber + "\r \n " + "**Nominee Details: **" +
                            completed.NomineeDetails+"\r \n "+"**Balance: **"+completed.SavingsAmount);
                        //store the entire bot conversation into azure sql database.
                        SQLDatabaseService.InsertAccountantInformation(completed,accno,accpin);
                        await context.PostAsync("Thanks for creating account here!");
                    }
                    catch (FormCanceledException<Customer> e)
                    {
                        string reply;
                        if (e.InnerException == null)
                        {
                            reply = $"You quit on {e.Last}--maybe you can finish next time!";
                        }
                        else
                        {
                            reply = "Sorry, I've had a short circuit.  Please try again.";
                        }
                        await context.PostAsync(reply);
                    }
                });
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        #region Bot_OldVersion_Code

        //public async Task<Message> Post([FromBody]Message message)
        //{

        //    if (message.Type == "Message")
        //    {    

        //            return await Conversation.SendAsync(message, MakeRootDialog);                       
        //    }
        //    else
        //    {
        //        return HandleSystemMessage(message);
        //    }
        //}

        //private Message HandleSystemMessage(Message message)
        //{
        //    if (message.Type == "Ping")
        //    {
        //        Message reply = message.CreateReplyMessage();
        //        reply.Type = "Ping";
        //        reply.Text = "Bot has been configured correctly with your current system.";
        //        return reply;
        //    }
        //    else if (message.Type == "DeleteUserData")
        //    {
        //        // Implement user deletion here
        //        // If we handle user deletion, return a real message
        //    }
        //    else if (message.Type == "BotAddedToConversation")
        //    {
        //        Message reply = message.CreateReplyMessage();
        //        reply.Type = "BotAddedToConversation";
        //        reply.Text = "Welcome to the IVR bot!";//\r \n Please enter any of these words like IVR,ivr....

        //        return reply;
        //    }
        //    else if (message.Type == "BotRemovedFromConversation")
        //    {
        //        Message reply = message.CreateReplyMessage();
        //        reply.Type = "BotRemovedFromConversation";
        //        reply.Text = "Your bot is removed from conversation";
        //        return reply;
        //    }
        //    else if (message.Type == "UserAddedToConversation")
        //    {
        //        Message reply = message.CreateReplyMessage();
        //        reply.Type = "UserAddedToConversation";
        //        reply.Text = "Successfully new user has been added to a conversation";
        //        return reply;
        //    }
        //    else if (message.Type == "UserRemovedFromConversation")
        //    {
        //        Message reply = message.CreateReplyMessage();
        //        reply.Type = "UserRemovedFromConversation";
        //        reply.Text = "Successfully user has been removed from conversation";
        //        return reply;
        //    }
        //    else if (message.Type == "EndOfConversation")
        //    {
        //        Message reply = message.CreateReplyMessage();
        //        reply.Type = "EndOfConversation";
        //        reply.Text = "Your conversation with bot has been be completed.";
        //        return reply;
        //    }
        //    else
        //    {
        //        Message reply = message.CreateReplyMessage();
        //        reply.Type = "Message";
        //        reply.Text = "I'm sorry. I didn't understand you.\r \n Please enter any of these words like IVR,ivr....";
        //        return reply;
        //    }
        //    return null;
        //}

        #endregion

        #region Bot_NewVersion(V3)_Code

        [ResponseType(typeof(void))]
       
        public virtual async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity != null)
            {
                
                StateClient  stateClient = activity.GetStateClient();
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                #region Check Authorization
                try
                {
                    //var d = await stateClient.BotState.GetUserDataAsync(Constants.Constants.botId, activity.From.Id);
                    //AuthenticationResult ar = AuthenticationResult.Deserialize(d.Data.ToString());
                    //AuthenticationContext ac = new AuthenticationContext("https://login.windows.net/common/oauth2/authorize/");
                    //ar = DateTimeOffset.Compare(DateTimeOffset.Now, ar.ExpiresOn) < 0 ? ar : await ac.AcquireTokenByRefreshTokenAsync(ar.RefreshToken, new ClientCredential(Constants.Constants.ADClientId, Constants.Constants.ADClientSecret));

                    // one of these will have an interface and process it
                    switch (activity.GetActivityType())
                    {
                        case ActivityTypes.Message:

                            await Conversation.SendAsync(activity, MakeRootDialog);
                            break;

                        case ActivityTypes.ConversationUpdate:
                            IConversationUpdateActivity conversationupdate = activity;
                            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                            {
                                var client = scope.Resolve<IConnectorClient>();
                                if (conversationupdate.MembersAdded.Any())
                                {
                                    var reply = activity.CreateReply();
                                    foreach (var newMember in conversationupdate.MembersAdded)
                                    {
                                        if (newMember.Id != activity.Recipient.Id)
                                        {
                                            reply.Text = $"Welcome {newMember.Name}! ";
                                        }
                                        else
                                        {
                                            reply.Text = $"Welcome {activity.From.Name}";
                                        }
                                        await client.Conversations.ReplyToActivityAsync(reply);
                                    }
                                }
                            }
                            break;
                        case ActivityTypes.ContactRelationUpdate:
                            IContactRelationUpdateActivity update = activity;
                            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                            {
                                var client = scope.Resolve<IConnectorClient>();
                                if (update.Action.ToLower() == "add")
                                {
                                    var reply = activity.CreateReply();
                                    reply.Text = $"Welcome to the BankIVR bot! To start an conversation with this bot send **IVR** or **ivr** command.\r \n if you need help, send the **Help** command.";
                                    await client.Conversations.ReplyToActivityAsync(reply);
                                }
                            }
                            break;
                        case ActivityTypes.Typing:
                        case ActivityTypes.DeleteUserData:
                        case ActivityTypes.Ping:
                        default:
                            Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                            break;
                    }

                }
                catch (Exception ex)
                {

                    //var query = "http://localhost:3978/api/{activity.From.Id}/login";
                    // var modifiedquery = query.Replace("x", "xxx").Replace("y", "xxy").Replace(":", "xyy");

                    //var reply = activity.CreateReply($"You must authenticate to use bot:"+ modifiedquery);

                    //var modifiedquery = activity.From.Id;
                    //var reply = activity.CreateReply($"You must authenticate to use bot: http://localhost:3978/api/{modifiedquery}/login");
                    //await connector.Conversations.ReplyToActivityAsync(reply);


                    //var query = activity.From.Id;
                    //var modifiedquery = query.Replace(":", "-");

                    //var reply = activity.CreateReply("Welcome to the Bank IVR bot!" + "(Hi)");
                    //await connector.Conversations.ReplyToActivityAsync(reply);
                    //var reply1 = activity.CreateReply($"You must authenticate to use bot: https://ivrbot.azurewebsites.net/api/{modifiedquery}/login");
                    //await connector.Conversations.ReplyToActivityAsync(reply1);
                }

                #endregion
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
           
        }

        
        #endregion

    }
}