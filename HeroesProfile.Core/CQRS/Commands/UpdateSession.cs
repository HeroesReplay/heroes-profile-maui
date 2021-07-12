using System.Threading;
using System.Threading.Tasks;
using Heroes.ReplayParser;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class UpdateSession
    {
        public record Command(Replay Replay, ParseType ParseType) : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly SessionRepository repository;

            public Handler(SessionRepository repository)
            {
                this.repository = repository;
            }

            public Task<MediatR.Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                repository.Set(request.Replay, request.ParseType);

                return Task.FromResult(MediatR.Unit.Value);
            }
        }

    }
}