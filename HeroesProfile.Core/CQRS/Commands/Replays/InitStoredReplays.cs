using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands.Replays;

public static class InitStoredReplays
{
    public record Command : IRequest;

    public class Handler : IRequestHandler<Command>
    {
        private readonly ReplaysRepository repository;

        public Handler(ReplaysRepository repository)
        {
            this.repository = repository;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await repository.InitAsync(cancellationToken);
        }
    }
}
