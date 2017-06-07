using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.ProjectOxford.Linguistics;
using System.Text;
using System.Collections.Generic;
using IntentProcessing.Contract;
using Microsoft.ApplicationInsights;

namespace IntentProcessing
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public static TelemetryClient telemetry = new TelemetryClient();

        #region private members

        /// <summary>
        /// The Default Service Host
        /// </summary>
        private const string DefaultServiceHost = "https://api.projectoxford.ai/linguistics/v1.0";

        /// <summary>
        /// The JSON content type header.
        /// </summary>
        private const string JsonContentTypeHeader = "application/json";

        /// <summary>
        /// The subscription key name.
        /// </summary>
        private const string SubscriptionKeyName = "ocp-apim-subscription-key";

        /// <summary>
        /// The ListAnalyzers.
        /// </summary>
        private const string ListAnalyzersQuery = "analyzers";

        /// <summary>
        /// The AnalyzeText.
        /// </summary>
        private const string AnalyzeTextQuery = "analyze";

        /// <summary>
        /// The default resolver.
        /// </summary>
        private static readonly CamelCasePropertyNamesContractResolver defaultResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// The settings
        /// </summary>
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = defaultResolver
        };

        /// <summary>
        /// The service host.
        /// </summary>
        private string serviceHost;

        /// <summary>
        /// The HTTP client
        /// </summary>
        private HttpClient httpClient;

        #endregion

        /// <summary>
        /// Default jsonserializer settings
        /// </summary>
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            var properties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","Post" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"User Spoken Message Json",message.ToString() } };

            telemetry.TrackEvent("Post Event Views", properties);

            string messagetext = message.Text;
            var aiproperties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","Post" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"User Spoken Message",messagetext } };

            telemetry.TrackEvent("Post Event Views", aiproperties);

            string resultsAsJson = "", botOutputString = "";
            this.serviceHost = string.IsNullOrWhiteSpace(serviceHost) ? DefaultServiceHost : serviceHost.Trim();

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(SubscriptionKeyName, "b7ba08bf576747728ad0a74af2d5718f");

            // List analyzers
            Analyzer[] supportedAnalyzers = null;
            try
            {
                var requestUrl = $"{this.serviceHost}/{ListAnalyzersQuery}";

                supportedAnalyzers = await SendRequestAsync<object, Analyzer[]>(HttpMethod.Get, requestUrl);
                var analyzersAsJson = JsonConvert.SerializeObject(supportedAnalyzers, Formatting.Indented, jsonSerializerSettings);
                //Console.WriteLine("Supported analyzers: " + analyzersAsJson);
            }
            catch (Exception e)
            {
                //Console.Error.WriteLine("Failed to list supported analyzers: " + e.ToString());
                Environment.Exit(1);
            }

            // Analyze text with all available analyzers
            var analyzeTextRequest = new AnalyzeTextRequest()
            {
                Language = "en",
                AnalyzerIds = supportedAnalyzers.Select(analyzer => analyzer.Id).ToArray(),
                Text = messagetext
            };

            try
            {
                var requestUrl = $"{this.serviceHost}/{AnalyzeTextQuery}";

                var analyzeTextResults = await this.SendRequestAsync<object, AnalyzeTextResult[]>(HttpMethod.Post, requestUrl, analyzeTextRequest);

                resultsAsJson = JsonConvert.SerializeObject(analyzeTextResults, Formatting.Indented, jsonSerializerSettings);

                //Console.WriteLine("Analyze text results: " + resultsAsJson);
                var insightproperties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","Post" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"Json Result",resultsAsJson } };

                telemetry.TrackEvent("Post Event Views", insightproperties);
            }
            catch (Exception e)
            {
                //Console.Error.WriteLine("Failed to list supported analyzers: " + e.ToString());
                Environment.Exit(1);
            }

            var data = JsonConvert.DeserializeObject<List<RootObject>>(resultsAsJson);

            if (data.Count == 3)
            {
                var jsonTreeList = data[0].result.ToArray();
                string jsonTree = jsonTreeList.Count() > 0 ? "{Nodes:" + jsonTreeList[0].ToString() + "}" : null;
                //jsonTree = "{Nodes:" + jsonTree;
                var posTags = JsonConvert.DeserializeObject<Tree>(jsonTree);


                var jsonTreeView = data[1].result.ToArray();

                var tokenList = data[2].result.ToArray();
                string tokenJson = tokenList.Count() > 0 ? tokenList[0].ToString() : null;
                var tokenData = JsonConvert.DeserializeObject<TokenRootObject>(tokenJson);

                for (int i = 0; i < posTags.Nodes.Count; i++)
                {
                    if (posTags.Nodes[i] == "NNP")
                    {

                        botOutputString += tokenData.Tokens[i].RawToken + " is Noun" + " \r \n";
                    }
                    else if (posTags.Nodes[i] == "VBG" || posTags.Nodes[i] == "VB")
                    {
                        botOutputString += tokenData.Tokens[i].RawToken + " is Verb" + " \r \n";
                    }
                    else if (posTags.Nodes[i] == "WRB")
                    {
                        botOutputString += tokenData.Tokens[i].RawToken + " is Adverb" + " \r \n";
                    }
                    else if (posTags.Nodes[i] == "WP")
                    {
                        botOutputString += tokenData.Tokens[i].RawToken + " is Pronoun" + " \r \n";
                    }
                    else if (posTags.Nodes[i] == "JJ" || posTags.Nodes[i] == "JJR" || posTags.Nodes[i] == "JJS")
                    {
                        botOutputString += tokenData.Tokens[i].RawToken + " is Adjective" + " \r \n";
                    }
                    else if (posTags.Nodes[i] == "IN")
                    {
                        botOutputString += tokenData.Tokens[i].RawToken + " is Preposition" + " \r \n";
                    }
                }

                botOutputString = botOutputString != "" ? "Speech and Natural Language Processing \r \n" + botOutputString : "";

                var insightproperties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","Post" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"Result From Linguistic API",botOutputString } };

                telemetry.TrackEvent("Post Event Views", insightproperties);

            }
            else
            {
                botOutputString = "";
            }

            

            //To identify name of a person, place and Company - Using LUIS
            var luisOutputString = "Intent and Language Understanding Intelligence Service Processing results are \r \n";
            var luisRequestURL = "https://api.projectoxford.ai/luis/v1/application?id=fbec04e7-8bda-4160-a059-a8f8b995184b&subscription-key=d14817bff85b4de0af2cc701b2e5de70";
            httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(luisRequestURL + "&q=" + messagetext);

            string luisResponseString = await response.Content.ReadAsStringAsync();

            var insightsproperties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","Post" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"Json Result From LUIS",luisResponseString } };

            telemetry.TrackEvent("Post Event Views", insightsproperties);

            var luisResponse = JsonConvert.DeserializeObject<LuisResponse>(luisResponseString);

            if (luisResponse.entities.Count > 0)
            {
                foreach (var entity in luisResponse.entities)
                {
                    if (entity.type.Contains("geography"))
                    {
                        if(!luisOutputString.ToLower().Contains(entity.entity.ToLower()))
                        luisOutputString += entity.type.Replace("builtin.geography.", "")+" : " + entity.entity + " \r \n";
                    }
                    else if (entity.type == "Name")
                    {
                        luisOutputString += "Name: " + entity.entity + " \r \n";
                    }
                    else if (entity.type == "Company")
                    {
                        luisOutputString += "Company: " + entity.entity + " \r \n";
                    }
                    else
                    {
                        luisOutputString += entity.type + " " + entity.entity + " \r \n";
                    }
                }
            }
            else
            {
                luisOutputString = "No matching found for Intent and Language Understanding Intelligence Service Processing";
            }

            if (botOutputString == "")
            {
                botOutputString = "No matching found for Natural Speech and Intent Processing";
            }

            var appinsightsproperties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","Post" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"Final Result From LUIS",luisOutputString } };

            telemetry.TrackEvent("Post Event Views", appinsightsproperties);

            return message.CreateReplyMessage(botOutputString + " \r \n \r \n \r \n \r \n" + luisOutputString);

        }

      

        #region the json client
        /// <summary>
        /// Sends the request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>The response.</returns>
        /// <exception cref="ClientException">The client exception.</exception>
        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod httpMethod, string requestUrl, TRequest requestBody = default(TRequest))
        {
            var request = new HttpRequestMessage(httpMethod, requestUrl);
            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, settings), Encoding.UTF8, JsonContentTypeHeader);
            }

            HttpResponseMessage response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }

                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    return JsonConvert.DeserializeObject<TResponse>(responseContent, settings);
                }
                var properties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","SendRequestAsync" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"Http Response",response.IsSuccessStatusCode.ToString() } };

                telemetry.TrackEvent("SendRequestAsync Event Views", properties);

                return default(TResponse);
            }
            else
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains(JsonContentTypeHeader))
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    ClientError errorCollection = JsonConvert.DeserializeObject<ClientError>(errorObjectString);
                    if (errorCollection != null)
                    {
                        throw new ClientException(errorCollection, response.StatusCode);
                    }
                }
                var properties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","SendRequestAsync" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"Http Response",response.IsSuccessStatusCode.ToString() } };

                telemetry.TrackEvent("SendRequestAsync Event Views", properties);

                response.EnsureSuccessStatusCode();
            }

            return default(TResponse);
        }
        #endregion

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
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
            var properties = new Dictionary<string, string> { {"Page Name","MessagesController" }, {"Method Name","HandleSystemMessage" },
                                                            { "Session Id",telemetry.Context.Session.Id }, {"Message Type",message.Type } };

            telemetry.TrackEvent("HandleSystemMessage Event Views", properties);
            return null;
        }
    }
}