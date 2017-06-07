using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using System.Net;
using Newtonsoft.Json.Linq;

namespace NearByBotApplication
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public static RootObject rootObject;
        //Geolocator _geolocator;
        //Geoposition pos;
            HttpClient http;
            string list = "";
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        public class RootObject
        {
            public List<object> html_attributions { get; set; }
            public string next_page_token { get; set; }
            public List<Result> results { get; set; }
            public string status { get; set; }
        }

        public class Result
        {
            public Geometry geometry { get; set; }
            public string icon { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string reference { get; set; }
            public List<string> types { get; set; }
            public string vicinity { get; set; }
          //  public OpeningHours opening_hours { get; set; }

            public string distance { get; set; }
        }

        public class LatLon
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
        public class Geometry
        {
            public LatLon location { get; set; }
        }
        string words;
        public static readonly IDialog<string> dialog = Chain.PostToChain()
            .Select(msg => msg.Text)
            .Switch(
             new DefaultCase<string, IDialog<string>>((context, txt) =>
             {
                 int count;
                 context.UserData.TryGetValue("count", out count);
                 context.UserData.SetValue("count", ++count);
                 string reply = string.Format("{0}: You said {1}", count, txt);
                 return Chain.Return(reply);
             }))
             .Unwrap()
            .PostToUser();
        string[] letters;
        //public async Task<Message> Post([FromBody]Message message)
        //{

        //    string messagetext = (message.Text).ToLower();

        //        await NearByPlaces(messagetext, 2 * 1000);
        //    if (rootObject != null)
        //    {
        //        if (rootObject.results.Count > 0)
        //        {
        //            foreach (var item2 in rootObject.results)
        //            {
        //                // return our reply to the user
        //                //item=  item1.Title;
        //                list += item2.name + "," + "\r \n";
        //            }

        //        }
        //        else
        //        {
        //            return message.CreateReplyMessage("Sorry we are unable to find the results for " +"''"+messagetext+"''"+ "Please make sure that you have typed correct phrase..." + "\r \n" + " some examples are..." + "\r \n" + "''" + "Restaurants in Albany" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "show me book stores in Norwich" + "''" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "Parking near Norwich" + "''" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "atms surrounding Norwich" + "''" + "''");
        //        }
        //    }
        //    else
        //    {
        //        return message.CreateReplyMessage("Oops.... Something went wrong please try again." );
        //    }
        //        //}
        //        return message.CreateReplyMessage(list);
        //    //}
        //    //else
        //    //{
        //       // return message.CreateReplyMessage("Oops.... Something went wrong. Please make sure that you have type praser..." + "\r \n" + " some examples are..." + "\r \n" + "''" + "show me nearby atm" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "show me nearby hospital" + "''");
        //        //}
        //   // }
        //}

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            string messagetext = (activity.Text).ToLower();
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            var data = userData.GetProperty<string>("searchedText");
            if(data!=null)
            {
                Activity reply = activity.CreateReply($"you previous searched phrase is" +"\r \n"+ $"**{data}**" + "\r \n");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            userData.SetProperty<string>("searchedText",activity.Text);
            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
            await NearByPlaces(messagetext, 2 * 1000);
            if (rootObject != null)
            {
                if (rootObject.results.Count > 0)
                {
                    foreach (var item2 in rootObject.results)
                    {
                        // return our reply to the user
                        //item=  item1.Title;
                        list += item2.name + "," + "\r \n";
                    }
                    Activity reply1 = activity.CreateReply(list);
                    await connector.Conversations.ReplyToActivityAsync(reply1);
                    
                }
                else
                {
                    Activity errorreply = activity.CreateReply("Sorry we are unable to find the results for " + "''" + messagetext + "''" + "Please make sure that you have typed correct phrase..." + "\r \n" + " some examples are..." + "\r \n" + "''" + "Restaurants in Albany" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "show me book stores in Norwich" + "''" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "Parking near Norwich" + "''" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "atms surrounding Norwich" + "''" + "''");
                    await connector.Conversations.ReplyToActivityAsync(errorreply);
                    // return Request.CreateResponse("Sorry we are unable to find the results for " + "''" + messagetext + "''" + "Please make sure that you have typed correct phrase..." + "\r \n" + " some examples are..." + "\r \n" + "''" + "Restaurants in Albany" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "show me book stores in Norwich" + "''" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "Parking near Norwich" + "''" + "''" + "\r \n" + "(or)" + "\r \n" + "''" + "atms surrounding Norwich" + "''" + "''");
                }
            }

            else
            {
                Activity errorreply = activity.CreateReply($"**Oops.... Something went wrong please try again.**");
                await connector.Conversations.ReplyToActivityAsync(errorreply);
                // return Request.CreateResponse("Oops.... Something went wrong please try again.");
            }
        
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        private async Task NearByPlaces(string message, double radiusInMeters)
        {
            rootObject = new RootObject();
            try
              {
                
                http = new HttpClient();
                HttpResponseMessage response = await http.GetAsync(new Uri("https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + message +"&key=AIzaSyBVPg6I5FPo8lrZLalbDFQA_OsGhv-XAk4"));

                var str = await response.Content.ReadAsStringAsync();

                if (str != null && str != "")
                {
                    rootObject = JsonConvert.DeserializeObject<RootObject>(await response.Content.ReadAsStringAsync());
                   
                }
              }
                catch (Exception ex)
                {
                //  telemetry.TrackException(ex);
                //var MEProperties = new Dictionary<object, object> {
                //{ "Page Name", "ItemDetailPage" },
                //{ "MethodName", "NearByPlaces" },
                //{ "Message", ex.Message } };
                //EngagementAgent.Instance.SendError("error", MEProperties);
                
                }
        }
    }
}