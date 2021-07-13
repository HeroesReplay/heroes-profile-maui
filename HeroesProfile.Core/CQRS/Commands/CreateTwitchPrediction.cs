using System;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions;
using TwitchLib.Api.Helix.Models.Predictions.CreatePrediction;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;
using TwitchLib.Api.Interfaces;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class CreateTwitchPrediction
    {
        public record Command : IRequest<Response>;

        public record Response(Prediction[] Predictions);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly ITwitchAPI twitchApi;
            private readonly Settings settings;
            private readonly SessionRepository sessionRepository;
            private readonly UserSettingsRepository settingsRepository;

            public Handler(ITwitchAPI twitchApi, Settings settings, SessionRepository sessionRepository, UserSettingsRepository settingsRepository)
            {
                this.twitchApi = twitchApi;
                this.settings = settings;
                this.sessionRepository = sessionRepository;
                this.settingsRepository = settingsRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                Session session = sessionRepository.GetSession();
                UserSettings userSettings = await settingsRepository.LoadAsync(cancellationToken);

                if (settings.EnableFakePrediction)
                {
                    session.TwitchPredictionId = Guid.NewGuid().ToString();
                    session.TwitchPredictionWinningOutcomeId = Guid.NewGuid().ToString();
                    sessionRepository.Set(session);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(session.TwitchPredictionId))
                    {
                        EndPredictionResponse endPredictionResponse = await twitchApi.Helix.Predictions.EndPrediction(userSettings.BroadcasterId, session.TwitchPredictionId, PredictionStatusEnum.CANCELED);
                    }

                    CreatePredictionRequest createPredictionRequest = new()
                    {
                        BroadcasterId = userSettings.BroadcasterId,
                        PredictionWindowSeconds = settings.PredictionWindowSeconds,
                        Title = "Heroes Profile Auto Predictions",
                        Outcomes = new TwitchLib.Api.Helix.Models.Predictions.CreatePrediction.Outcome[] { new() { Title = "Win" }, new() { Title = "Loss" } }
                    };

                    CreatePredictionResponse createPredictionResponse = await twitchApi.Helix.Predictions.CreatePrediction(createPredictionRequest, userSettings.TwitchAccessToken);

                    if (createPredictionResponse.Data != null && createPredictionResponse.Data.Length == 1)
                    {
                        session.TwitchPredictionId = createPredictionResponse.Data[0].Id;
                        session.TwitchPredictionWinningOutcomeId = createPredictionResponse.Data[0].WinningOutcomeId;
                        session.TwitchPredictionOtherOutcomeId = createPredictionResponse.Data[1].Outcomes[1].Id;
                    }

                    return new Response(createPredictionResponse.Data);
                }

                return new Response(Array.Empty<Prediction>());
            }
        }
    }
}