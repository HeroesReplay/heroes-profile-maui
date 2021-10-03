using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Clients;

using MediatR;

namespace MauiApp2.Core.CQRS.Commands.Discord
{
    public static class ClearActivity
    {
        public record Command() : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly DiscordClient discordClient;

            public Handler(DiscordClient discordClient)
            {
                this.discordClient = discordClient;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                discordClient.ClearActivity();
                return Unit.Value;
            }
        }
    }
}