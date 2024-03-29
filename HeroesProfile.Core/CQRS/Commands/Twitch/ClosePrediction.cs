﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;

namespace HeroesProfile.Core.CQRS.Commands.Twitch;

public static class ClosePrediction
{
    public record Command : IRequest<Response>;

    public record Response(bool Won, string OutcomeId);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly SessionRepository sessionRepository;
        private readonly IMediator mediator;
        private readonly UserSettingsRepository settingsRepository;
        private readonly AppSettings appSettings;
        private readonly PredictionsClient predictionClient;

        private SessionData session => sessionRepository.SessionData;

        public Handler(IMediator mediator, UserSettingsRepository settingsRepository, SessionRepository sessionRepository, AppSettings appSettings, PredictionsClient predictionsClient)
        {
            this.mediator = mediator;
            this.settingsRepository = settingsRepository;
            this.sessionRepository = sessionRepository;
            this.appSettings = appSettings;
            predictionClient = predictionsClient;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var userSettings = await settingsRepository.LoadAsync(cancellationToken);

            if (session.StormReplay != null && session.Prediction != null)
            {
                GetKnownBattleNetIds.Response knownBattleNetIds = await mediator.Send(new GetKnownBattleNetIds.Query());

                bool isWon = session.StormReplay.Players.Any(p => knownBattleNetIds.BattleNetIds.Any(battleNetId => p.IsWinner && p.BattleNetId == battleNetId));

                string outcomeId = isWon ? session.Prediction.WinningOutcomeId : session.Prediction.OtherOutcomeId;

                EndPredictionResponse response = await predictionClient.EndPrediction(userSettings.Identity, session.Prediction.PredictionId, PredictionEndStatus.RESOLVED, outcomeId, cancellationToken);

                await mediator.Publish(new TwitchPredictionUpdated.Notification(sessionRepository.SessionData), cancellationToken);

                return new Response(isWon, outcomeId);
            }

            return new Response(false, string.Empty);
        }
    }
}