using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class InitializeDefaultUserSettings
    {
        public record Command : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly UserSettingsRepository repository;
            private readonly UserSettings defaultUserSettings;

            public Handler(UserSettingsRepository repository, UserSettings defaultUserSettings)
            {
                this.repository = repository;
                this.defaultUserSettings = defaultUserSettings;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await repository.SaveAsync(defaultUserSettings, cancellationToken);

                return Unit.Value;
            }
        }
    }
}