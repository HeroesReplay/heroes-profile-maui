using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class InitializeUserSettings
    {
        public record Command : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly UserSettingsRepository repository;

            public Handler(UserSettingsRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await repository.InitilizeAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}