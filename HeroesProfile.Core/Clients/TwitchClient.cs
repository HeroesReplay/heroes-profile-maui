using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Models;

using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions.CreatePrediction;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;
using TwitchLib.Api.Interfaces;

namespace MauiApp2.Core.Clients
{
    public class PredictionsClient
    {
        private readonly AppSettings appSettings;
        private readonly HttpClient httpClient;

        public static readonly Uri CreatePredictionUri = new("twitch/predictions/create", UriKind.Relative);
        public static readonly Uri EndPredictionUri = new("twitch/predictions/end", UriKind.Relative);


        public PredictionsClient(AppSettings appSettings, HttpClient httpClient)
        {
            this.appSettings = appSettings;
            this.httpClient = httpClient;
        }

        public async Task<CreatePredictionResponse> CreatePrediction(Dictionary<string, string> identity, CreatePredictionRequest request, CancellationToken cancellationToken)
        {
            var values = new Dictionary<string, string>(identity)
            {
                { "BroadcasterId", request.BroadcasterId },
                { "Outcome1", request.Outcomes[0].Title },
                { "Outcome2", request.Outcomes[1].Title }
            };

            using (var content = new FormUrlEncodedContent(values))
            {
                HttpResponseMessage response = await httpClient.PostAsync(CreatePredictionUri, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return new CreatePredictionResponse(); // deserialize Heroes Profile Response
                }
            }

            return new CreatePredictionResponse();
        }

        public async Task<EndPredictionResponse> EndPrediction(Dictionary<string, string> identity, string predictionId, PredictionStatusEnum status, string outcomeId, CancellationToken cancellationToken)
        {
            var values = new Dictionary<string, string>(identity)
            {
                { "predictionId", predictionId },
                { "status", status.ToString() }, // RESOLVED, CANCELED, LOCKED
                { "outcomeId", outcomeId }
            };

            using (var content = new FormUrlEncodedContent(values))
            {
                HttpResponseMessage response = await httpClient.PostAsync(EndPredictionUri, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return new EndPredictionResponse(); // deserialize Heroes Profile Response
                }
            }

            return new EndPredictionResponse();
        }
    }
}
