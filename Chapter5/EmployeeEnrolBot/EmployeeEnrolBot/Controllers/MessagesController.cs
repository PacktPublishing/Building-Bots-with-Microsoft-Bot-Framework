using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;

namespace EmployeeEnrolBot
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
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                StateClient stateClient = activity.GetStateClient();
                Query Result = new Query();

                try
                {
                    if (activity.Text != null)
                    {
                        string strContextId = "";
                        BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                        
                        if (userData.GetProperty<string>("contextId")!=null)
                        {
                            // If we have a ContextId saved in TempData
                            // retrieve it
                            strContextId = userData.GetProperty<string>("contextId").ToString();
                        }

                        LUIS objLUISResult = await QueryLUIS(activity.Text,strContextId);
                        if (objLUISResult.dialog.prompt != null)
                        {
                            // If there is a question ask it
                            Result.Question = objLUISResult.dialog.prompt;
                            // Set the ContextID
                            userData.SetProperty<string>("contextId", objLUISResult.dialog.contextId);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                            // return our reply to the user
                            Activity reply = activity.CreateReply(Result.Question);
                            await connector.Conversations.ReplyToActivityAsync(reply);
                        }
                        else
                        {
                            userData.SetProperty<string>("contextId", "");
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            foreach (var item in objLUISResult.topScoringIntent.actions)
                            {
                                // Loop through the parameters
                                foreach (var parameter in item.parameters)
                                {
                                    if (parameter.value[0].type == "Employee Name::First Name")
                                    {
                                        Result.FirstName = parameter.value[0].entity;
                                    }

                                    if (parameter.value[0].type == "Employee Name::Last Name")
                                    {
                                        Result.LastName = parameter.value[0].entity;
                                    }

                                    if (parameter.value[0].type == "Department")
                                    {
                                        Result.Department = parameter.value[0].entity;
                                    }

                                    if (parameter.value[0].type == "Designation")
                                    {
                                        Result.Designation = parameter.value[0].entity;
                                    }
                                }
                            }
                            // return our reply to the user
                            Activity reply = activity.CreateReply($"Employee First Name: {Result.FirstName} \r \n Employee Last Name: {Result.LastName} \r \n Department: {Result.Department} \r \n Designation:{Result.Designation}");
                            await connector.Conversations.ReplyToActivityAsync(reply);
                        }
                       
                    }
                    
                }
                catch (Exception ex)
                {
                    
                    // return our reply to the user
                    Activity reply = activity.CreateReply($"Something went wrong. \r \n"+ex.Message);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                                

            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
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

            return null;
        }

        // Utility

        #region 
        private static async Task<LUIS> QueryLUIS(string Query, string contextId)
        {
            // Create a new LUIS class
            LUIS LUISResult = new LUIS();

            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                // Get key values from the web.config
                string LUIS_Url =
                    ConfigurationManager.AppSettings["LUIS_Url"];
                string LUIS_Id =
                    ConfigurationManager.AppSettings["LUIS_APP_Id"];
                string LUIS_Subscription_Key =
                    ConfigurationManager.AppSettings["LUIS_Subscription_Key"];

                // Get the text of the query entered by the user
                var LUISQuery = Uri.EscapeDataString(Query);

                // Send Query to LUIS and get response
                string RequestURI = String.Format("{0}?id={1}&subscription-key={2}&q={3}&contextId={4}",
                    LUIS_Url, LUIS_Id, LUIS_Subscription_Key, LUISQuery, contextId);

                System.Net.Http.HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    LUISResult = JsonConvert.DeserializeObject<LUIS>(JsonDataResponse);
                }
            }

            return LUISResult;
        }
        #endregion
    }
}