using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions.EndPrediction;
using TwitchLib.Api.Interfaces;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class CloseTwitchPrediction
    {
        public record Command : IRequest<Response>;

        public record Response(bool Won, string OutcomeId);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly SessionRepository sessionRepository;
            private readonly UserSettingsRepository settingsRepository;
            private readonly Settings settings;
            private readonly ITwitchAPI twitchApi;

            public Handler(UserSettingsRepository settingsRepository, SessionRepository sessionRepository, Settings settings, ITwitchAPI twitchApi)
            {
                this.settingsRepository = settingsRepository;
                this.sessionRepository = sessionRepository;
                this.settings = settings;
                this.twitchApi = twitchApi;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                Session session = sessionRepository.GetSession();
                UserSettings userSettings = await settingsRepository.LoadAsync(cancellationToken);

                if (session.StormReplay != null && session.TwitchPredictionId != null)
                {
                    bool isWon = settings.EnableFakePrediction ? (DateTime.Now.Millisecond > 499) : session.StormReplay.Players.Any(p => userSettings.BattleTags.Any(storedBattleTag => $"{p.Name}#{p.BattleTag}" == storedBattleTag && p.IsWinner));
                    string outcomeId = isWon ? session.TwitchPredictionWinningOutcomeId : session.TwitchPredictionOtherOutcomeId;

                    if (settings.EnableFakePrediction)
                    {
                        return new(isWon, outcomeId);
                    }
                    else
                    {
                        EndPredictionResponse response = await twitchApi.Helix.Predictions.EndPrediction(userSettings.BroadcasterId, session.TwitchPredictionId, PredictionStatusEnum.RESOLVED, outcomeId, accessToken: userSettings.TwitchAccessToken);

                        // TODO: Do we need to do further analysis?
                        // response.Data[0].Outcomes[0].Id
                        // response.Data[0].Outcomes[0].TopPredictors[0].User
                        // response.Data[0].Outcomes[0].ChannelPoints

                        return new Response(isWon, outcomeId);
                    }
                }

                return new Response(false, string.Empty);
            }
        }
    }
}