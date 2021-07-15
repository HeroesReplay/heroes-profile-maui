using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Clients;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class ClosePrediction
    {
        public record Command : IRequest<Response>;

        public record Response(bool Won, string OutcomeId);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly SessionRepository sessionRepository;
            private readonly UserSettingsRepository settingsRepository;
            private readonly AppSettings appSettings;
            private readonly ITwitchWrapper twitchWrapper;

            private Session session => sessionRepository.Session;

            public Handler(UserSettingsRepository settingsRepository, SessionRepository sessionRepository, AppSettings appSettings, ITwitchWrapper twitchWrapper)
            {
                this.settingsRepository = settingsRepository;
                this.sessionRepository = sessionRepository;
                this.appSettings = appSettings;
                this.twitchWrapper = twitchWrapper;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {

                UserSettings userSettings = await settingsRepository.LoadAsync(cancellationToken);

                if (session.StormReplay != null && session.Prediction != null)
                {
                    bool isWon = appSettings.EnableFakeTwitch ? (DateTime.Now.Millisecond > 499) : session.StormReplay.Players.Any(p => userSettings.BattleTags.Any(storedBattleTag => $"{p.Name}#{p.BattleTag}" == storedBattleTag && p.IsWinner));
                    string outcomeId = isWon ? session.Prediction.WinningOutcomeId : session.Prediction.OtherOutcomeId;

                    EndPredictionResponse response = await twitchWrapper.EndPrediction(userSettings.BroadcasterId, session.Prediction.PredictionId, PredictionStatusEnum.RESOLVED, outcomeId, accessToken: userSettings.TwitchAccessToken);

                    return new Response(isWon, outcomeId);
                }

                return new Response(false, string.Empty);
            }
        }
    }
}