using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions.CreatePrediction;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;
using TwitchLib.Api.Interfaces;

namespace HeroesProfile.Core.Clients
{
    public interface ITwitchWrapper
    {
        Task<EndPredictionResponse> EndPrediction(string broadcasterId, string predictionId, PredictionStatusEnum status, string? outcomeId = null, string? accessToken = null);
        Task<CreatePredictionResponse> CreatePrediction(CreatePredictionRequest request, string? accessToken = null);
    }

    public class TwitchWrapperClient : ITwitchWrapper
    {
        private readonly ITwitchAPI twitchAPI;

        public TwitchWrapperClient(ITwitchAPI twitchAPI)
        {
            this.twitchAPI = twitchAPI;
        }

        public async Task<CreatePredictionResponse> CreatePrediction(CreatePredictionRequest request, string? accessToken = null)
        {
            return await twitchAPI.Helix.Predictions.CreatePrediction(request, accessToken);
        }

        public async Task<EndPredictionResponse> EndPrediction(string broadcasterId, string predictionId, PredictionStatusEnum status, string? outcomeId = null, string? accessToken = null)
        {
            return await twitchAPI.Helix.Predictions.EndPrediction(broadcasterId, predictionId, status, outcomeId, accessToken);
        }
    }
}
