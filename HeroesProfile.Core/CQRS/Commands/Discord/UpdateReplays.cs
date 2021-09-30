using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Clients;
using MauiApp2.Core.CQRS.Notifications;
using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Commands.Discord
{
    public static class UpdatePresence
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
                Queries.GetKnownBattleNetIds.Response? knownBattleNetIdsResponse = await this.mediator.Send(new Queries.GetKnownBattleNetIds.Query(), cancellationToken);
                Queries.GetSession.Response? sessionResponse = await this.mediator.Send(new Queries.GetSession.Query(), cancellationToken);

                discordClient.UpdatePresence(sessionResponse.Session, knownBattleNetIdsResponse.BattleNetIds);

                return Unit.Value;
            }
        }
    }
}