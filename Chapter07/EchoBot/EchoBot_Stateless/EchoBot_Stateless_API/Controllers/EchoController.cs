using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EchoBot_Stateless_API.Controllers
{
    [BotAuthentication(MicrosoftAppId = "e3911b02-2819-47d6-bc99-48e696e7ca55", 
        MicrosoftAppPassword = "feeRqC9b2EjBa01NbDnRfw4")]
    public class EchoController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new string[] { "Service Fabric", "Echo - Stateless Microservice" };
        }
        /// <summary>
                /// POST: api/Messages
                /// Receive a message from a user and reply to it
                /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity message)
        {
            if (message.Type.ToLower() == "message")
            {
                var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                var reply = message.CreateReply($"Service Fabric knows you said : {message.Text}");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
               HandleSystemMessage(message);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == "Ping")
            {
                //Message reply = message.CreateReplyMessage();
                //reply.Type = "Ping";
                //return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }

    }
}
