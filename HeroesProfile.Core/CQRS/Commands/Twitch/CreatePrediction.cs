using System;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Clients;
using MauiApp2.Core.CQRS.Notifications;
using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions;
using TwitchLib.Api.Helix.Models.Predictions.CreatePrediction;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;
using TwitchLib.Api.Interfaces;

namespace MauiApp2.Core.CQRS.Commands.Twitch
{
    public static class CreatePrediction
    {
        public record Command : IRequest<Response>;

        public record Response(Prediction[] Predictions);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IMediator mediator;
            private readonly PredictionsClient predictionClient;
            private readonly AppSettings appSettings;
            private readonly SessionRepository sessionRepository;
            private readonly UserSettingsRepository settingsRepository;

            public Handler(IMediator mediator, PredictionsClient predictionsClient, AppSettings appSettings, SessionRepository sessionRepository, UserSettingsRepository settingsRepository)
            {
                this.mediator = mediator;
                predictionClient = predictionsClient;
                this.appSettings = appSettings;
                this.sessionRepository = sessionRepository;
                this.settingsRepository = settingsRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                Models.UserSettings userSettings = await settingsRepository.LoadAsync(cancellationToken);

                SessionData session = sessionRepository.SessionData;

                if (!string.IsNullOrWhiteSpace(session.Prediction?.PredictionId))
                {
                    EndPredictionResponse endPredictionResponse = await predictionClient.EndPrediction(userSettings.Identity, session.Prediction.PredictionId, PredictionStatusEnum.CANCELED, null, cancellationToken);
                }

                CreatePredictionRequest createPredictionRequest = new()
                {
                    BroadcasterId = userSettings.BroadcasterId,
                    PredictionWindowSeconds = appSettings.PredictionWindowSeconds,
                    Title = "Heroes Profile Auto Predictions",
                    Outcomes = new TwitchLib.Api.Helix.Models.Predictions.CreatePrediction.Outcome[] { new() { Title = "Win" }, new() { Title = "Loss" } }
                };

                CreatePredictionResponse createPredictionResponse = await predictionClient.CreatePrediction(userSettings.Identity, createPredictionRequest, cancellationToken);

                if (createPredictionResponse.Data != null && createPredictionResponse.Data.Length == 1)
                {
                    session.Prediction.PredictionId = createPredictionResponse.Data[0].Id;
                    session.Prediction.WinningOutcomeId = createPredictionResponse.Data[0].WinningOutcomeId;
                    session.Prediction.OtherOutcomeId = createPredictionResponse.Data[1].Outcomes[1].Id;
                    session.Prediction.LastUpdate = DateTime.Now;

                    await mediator.Publish(new TwitchPredictionUpdated.Notification(sessionRepository.SessionData), cancellationToken);
                }

                return new Response(createPredictionResponse.Data);
            }
        }
    }
}