using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands.Discord;


public static class SetActivity
{
    public record Command() : IRequest;

    public class Handler : IRequestHandler<Command>
    {
        private readonly IMediator mediator;
        private readonly DiscordClient discordClient;

        public Handler(IMediator mediator, DiscordClient discordClient)
        {
            this.mediator = mediator;
            this.discordClient = discordClient;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            GetKnownBattleNetIds.Response? knownBattleNetIdsResponse = await mediator.Send(new GetKnownBattleNetIds.Query(), cancellationToken);
            GetSession.Response? sessionResponse = await mediator.Send(new GetSession.Query(), cancellationToken);
            GetUserSettings.Response? userSettingsResponse = await mediator.Send(new GetUserSettings.Query(), cancellationToken);

            discordClient.UpdatePresence(sessionResponse.Session, userSettingsResponse.UserSettings, knownBattleNetIdsResponse.BattleNetIds);

            return Unit.Value;
        }
    }
}