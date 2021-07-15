using HeroesProfile.Core.Clients;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions;
using TwitchLib.Api.Helix.Models.Predictions.CreatePrediction;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;

namespace HeroesProfile.Core.Fakes
{
    public class FakeTwitchDelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }

    public class FakeOutcome : TwitchLib.Api.Helix.Models.Predictions.Outcome
    {
        public FakeOutcome(string id, string title)
        {
            Id = id;
            Title = title;
        }
    }

    public class FakeCreatePredictionResponse : CreatePredictionResponse
    {
        public FakeCreatePredictionResponse(TwitchPredictionData data)
        {
            this.Data = new Prediction[]
            {
                new FakePrediction(data),
            };
        }
    }

    public class FakePrediction : Prediction
    {
        public FakePrediction(TwitchPredictionData data)
        {
            this.Id = data.PredictionId;
            this.WinningOutcomeId = data.WinningOutcomeId;
            this.Outcomes = new TwitchLib.Api.Helix.Models.Predictions.Outcome[]
            {
                new FakeOutcome(WinningOutcomeId, "Win"),
                new FakeOutcome(data.OtherOutcomeId, "Loss")
            };
        }
    }

    public class FakeTwitchWrapperClient : ITwitchWrapper
    {
        private readonly SessionRepository sessionRepository;

        public FakeTwitchWrapperClient(SessionRepository sessionRepository)
        {
            this.sessionRepository = sessionRepository;
        }

        public Task<CreatePredictionResponse> CreatePrediction(CreatePredictionRequest request, string? accessToken = null)
        {
            sessionRepository.Session.Prediction.PredictionId = Guid.NewGuid().ToString();
            sessionRepository.Session.Prediction.WinningOutcomeId = Guid.NewGuid().ToString();
            sessionRepository.Session.Prediction.OtherOutcomeId = Guid.NewGuid().ToString();

            return Task.FromResult<CreatePredictionResponse>(new FakeCreatePredictionResponse(sessionRepository.Session.Prediction));
        }

        public Task<EndPredictionResponse> EndPrediction(string broadcasterId, string predictionId, PredictionStatusEnum status, string? outcomeId = null, string? accessToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
