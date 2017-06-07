using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Dialogs;
using System.Diagnostics;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

namespace WeatherBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                if (activity != null && activity.Type == ActivityTypes.Message)
                {
                    var text = (activity.Text).ToLower();
                    await Conversation.SendAsync(activity, () => new WeatherDialog());
                }
                else
                {
                    HandleSystemMessage(activity);
                }

                return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                var content = new StringContent(ex.Message);
                var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                responseMessage.Content = content;
                return responseMessage;
            }
            
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                IContactRelationUpdateActivity update = message;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    if (update.Action.ToLower() == "add")
                    {
                        var reply = message.CreateReply();
                        reply.Text = $"Welcome to the Temperature Bot! To start an conversation with this bot send **Get weather in Seattle ** or **GET WEATHER IN SEATTLE** command.\r \n if you need help, send the **Help** command.";
                        await client.Conversations.ReplyToActivityAsync(reply);
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            else
            {
                Trace.TraceError($"Unknown activity type ignored: {message.GetActivityType()}");
            }
            return null;
        }
        
    }
}