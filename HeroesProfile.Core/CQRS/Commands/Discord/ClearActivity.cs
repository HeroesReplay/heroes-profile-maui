using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Clients;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands.Discord;

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