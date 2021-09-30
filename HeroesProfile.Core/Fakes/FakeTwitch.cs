using MauiApp2.Core.Clients;
using MauiApp2.Core.Repositories;

using MauiApp2.Core.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions;
using TwitchLib.Api.Helix.Models.Predictions.CreatePrediction;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;

namespace MauiApp2.Core.Fakes
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
            Data = new Prediction[]
            {
                new FakePrediction(data),
            };
        }
    }

    public class FakeEndPredictionResponse : EndPredictionResponse
    {
        public FakeEndPredictionResponse(TwitchPredictionData data)
        {
            Data = new Prediction[]
            {
                new FakePrediction(data),
            };
        }
    }

    public class FakePrediction : Prediction
    {
        public FakePrediction(TwitchPredictionData data)
        {
            Id = data.PredictionId;
            WinningOutcomeId = data.WinningOutcomeId;
            Outcomes = new TwitchLib.Api.Helix.Models.Predictions.Outcome[]
            {
                new FakeOutcome(WinningOutcomeId, "Win"),
                new FakeOutcome(data.OtherOutcomeId, "Loss")
            };
        }
    }

    public class FakePreditionsClient
    {
        TwitchPredictionData data;

        public FakePreditionsClient()
        {
            data = new TwitchPredictionData()
            {
                OtherOutcomeId = Guid.NewGuid().ToString(),
                PredictionId = Guid.NewGuid().ToString(),
                WinningOutcomeId = Guid.NewGuid().ToString()
            };
        }


        public Task<CreatePredictionResponse> CreatePrediction(Dictionary<string, string> identity, CreatePredictionRequest request, CancellationToken cancellationToken)
        {
            var response = new FakeCreatePredictionResponse(data);

            return Task.FromResult<CreatePredictionResponse>(response);
        }


        public Task<EndPredictionResponse> EndPrediction(Dictionary<string, string> identity, string predictionId, PredictionStatusEnum status, string outcomeId, CancellationToken cancellationToken)
        {
            var response = new FakeEndPredictionResponse(data);
            return Task.FromResult<EndPredictionResponse>(response);
        }
    }
}
