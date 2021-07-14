using System;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions;
using TwitchLib.Api.Helix.Models.Predictions.CreatePrediction;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;
using TwitchLib.Api.Interfaces;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class CreatePrediction
    {
        public record Command : IRequest<Response>;

        public record Response(Prediction[] Predictions);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly ITwitchAPI twitchApi;
            private readonly AppSettings appSettings;
            private readonly SessionRepository sessionRepository;
            private readonly UserSettingsRepository settingsRepository;

            public Handler(ITwitchAPI twitchApi, AppSettings appSettings, SessionRepository sessionRepository, UserSettingsRepository settingsRepository)
            {
                this.twitchApi = twitchApi;
                this.appSettings = appSettings;
                this.sessionRepository = sessionRepository;
                this.settingsRepository = settingsRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                Models.UserSettings userSettings = await settingsRepository.LoadAsync(cancellationToken);

                Models.Session session = sessionRepository.Session;

                if (appSettings.EnableFakePrediction)
                {
                    session.Prediction.PredictionId = Guid.NewGuid().ToString();
                    session.Prediction.WinningOutcomeId = Guid.NewGuid().ToString();
                    session.Prediction.OtherOutcomeId = Guid.NewGuid().ToString();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(session.Prediction?.PredictionId))
                    {
                        EndPredictionResponse endPredictionResponse = await twitchApi.Helix.Predictions.EndPrediction(userSettings.BroadcasterId, session.Prediction.PredictionId, PredictionStatusEnum.CANCELED);
                    }

                    CreatePredictionRequest createPredictionRequest = new()
                    {
                        BroadcasterId = userSettings.BroadcasterId,
                        PredictionWindowSeconds = appSettings.PredictionWindowSeconds,
                        Title = "Heroes Profile Auto Predictions",
                        Outcomes = new TwitchLib.Api.Helix.Models.Predictions.CreatePrediction.Outcome[] { new() { Title = "Win" }, new() { Title = "Loss" } }
                    };

                    CreatePredictionResponse createPredictionResponse = await twitchApi.Helix.Predictions.CreatePrediction(createPredictionRequest, userSettings.TwitchAccessToken);

                    if (createPredictionResponse.Data != null && createPredictionResponse.Data.Length == 1)
                    {
                        session.Prediction.PredictionId = createPredictionResponse.Data[0].Id;
                        session.Prediction.WinningOutcomeId = createPredictionResponse.Data[0].WinningOutcomeId;
                        session.Prediction.OtherOutcomeId = createPredictionResponse.Data[1].Outcomes[1].Id;
                    }

                    return new Response(createPredictionResponse.Data);
                }

                return new Response(Array.Empty<Prediction>());
            }
        }
    }
}