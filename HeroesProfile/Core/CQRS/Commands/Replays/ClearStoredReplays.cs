using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Replays;

public static class ClearStoredReplays
{
    public record Command : IRequest;

    public class Handler(ReplaysRepository repository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await repository.ClearAsync(cancellationToken);
        }
    }
}