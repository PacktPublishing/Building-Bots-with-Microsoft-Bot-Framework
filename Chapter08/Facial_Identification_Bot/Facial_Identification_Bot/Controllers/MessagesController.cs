using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Net.Http.Headers;


namespace Facial_Identification_Bot
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
            var attachment = activity.Attachments?.FirstOrDefault();
            if (attachment?.ContentUrl != null)
            {
                using (var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl)))
                {
                    var token = await (connectorClient.Credentials as MicrosoftAppCredentials).GetTokenAsync();
                    var uri = new Uri(attachment.ContentUrl);
                    using (var httpClient = new HttpClient())
                    {
                        if (uri.Host.EndsWith("skype.com") && uri.Scheme == Uri.UriSchemeHttps)
                        {
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                        }
                        else
                        {
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(attachment.ContentType));
                        }

                        //var streamData = await httpClient.GetStreamAsync(uri);
                       
                       var emotions= await HelperClass.emotionAPIAnalysis(await httpClient.GetStreamAsync(uri));
                       var faceAttributes = await HelperClass.faceAPIAnalysis(await httpClient.GetStreamAsync(uri));

                        // return our reply to the user
                        Activity reply = activity.CreateReply($"**Face Analytics of given Image are** \r \n {faceAttributes} \r \n \r \n **Emotion Analytics of given image are** \r \n {emotions}");
                        await connectorClient.Conversations.ReplyToActivityAsync(reply);
                    }
                }
            }
            else  if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                
                // return our reply to the user
                Activity reply = activity.CreateReply($"**Please upload an image to see Face and Emotion Analytics.**");
                await connector.Conversations.ReplyToActivityAsync(reply);
               
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
               
    }

    
}