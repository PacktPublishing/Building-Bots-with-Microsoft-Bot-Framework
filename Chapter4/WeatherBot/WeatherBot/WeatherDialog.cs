using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace WeatherBot
{
    [Serializable]
    public class WeatherDialog: IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Object> argument)
        {
            var activity = await argument as Activity;
            string queryText = activity.Text;
            var locationInfo = await IdentifyCityUsingLUIS(queryText);
            var currentObservation = await GetCurrentWeatherUsingAPI(locationInfo);

            if (currentObservation != null)
            {
                string displayLocation = currentObservation.display_location?.full;
                decimal tempC = currentObservation.temp_c;
                string weather = currentObservation.weather;
                var weatherInfo = $"It is {weather} and {tempC} degrees in {displayLocation}.";
                string icon = currentObservation.icon;
                //string rfc822DateTime = currentObservation.observation_time_rfc822;
                //var observationTime = DateTime.Parse(rfc822DateTime);
                //var dayOrNight = observationTime.Hour;

                Activity replyToConversation = activity.CreateReply($"Weather report in {locationInfo} is");
                replyToConversation.Type = "message";
                replyToConversation.Attachments = new List<Attachment>();
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: "http://icons.wxug.com/i/c/g/" + icon + ".gif"));
                ThumbnailCard plCard = new ThumbnailCard()
                {
                    Text = weatherInfo,
                    Title = "Current Weather",
                    Images = cardImages,

                };
                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
                await context.PostAsync(replyToConversation);
            }
            else
            {
                await context.PostAsync($"There is more than one '{locationInfo}'. Can you be more specific?");
            }
            context.Wait(MessageReceivedAsync);
        }

      

      
        private static async Task<string> IdentifyCityUsingLUIS(string message)
        {
            using (var httpClient = new HttpClient())
            {
                var responseInString = await httpClient.GetStringAsync(@"https://api.projectoxford.ai/luis/v1/application?id=dc8551db-8e6d-48c3-a1f2-d44c8318c2f4&subscription-key=e634f566152d48b48ab09e1a919a1a37&q="
            + System.Uri.EscapeDataString(message));
               dynamic response = JObject.Parse(responseInString);

                var intent = response.intents?.First?.intent;
                string city="",state="",country="";
                if (intent == "getweather")
                {
                    foreach(var entity in response.entities)
                    {
                        if (entity.type == "builtin.geography.city")
                        {    
                            if(city=="")                        
                            city= entity.entity;
                            else
                            {
                                if(city==state)
                                {
                                    city = entity.entity;
                                }
                                else if(entity.entity == state)
                                {

                                }
                            }
                        }
                        else if (entity?.type == "builtin.geography.us_state")
                        {
                            state= entity.entity;
                        }
                        else if (entity?.type == "builtin.geography.country")
                        {
                            country= entity.entity;
                        }
                        
                    }
                    if (city != "" && state != "" && country != "")
                        return city + "," + state + "," + country;
                    else if(city != "" && state != "")
                        return city + "," + state;
                    else if (city != "" && country != "")
                        return city + "," + country;
                    else if (state != "" && country != "")
                        return state + "," + country;
                    else if (city != "")
                        return city;
                    else if (state != "")
                        return state;
                    else if (country != "")
                        return country;
                    else
                        return null;
                }

                return null;
            }
        }
        private static async Task<dynamic> GetCurrentWeatherUsingAPI(string location)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var escapedLocation = Regex.Replace(location, @"\W+", "_");
                    var jsonString = await client.GetStringAsync($"http://api.wunderground.com/api/90987b65e7b16cf8/conditions/q/{escapedLocation}.json");
                    dynamic response = JObject.Parse(jsonString);

                    //var jsonString = await client.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={escapedLocation}&APPID=4d6d226dc4d57d7911a170544803125d");

                    //response = JObject.Parse(jsonString);

                    dynamic observation = response.current_observation;
                    dynamic results = response.response.results;

                    if (observation != null)
                    {
                        return observation;
                        
                    }
                    else if (results != null)
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {

                }

                return null;
            }
        }
    }
}