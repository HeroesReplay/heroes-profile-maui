using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Commands.UserSettings
{
    public static class InitializeDefaultUserSettings
    {
        public record Command : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly UserSettingsRepository repository;
            private readonly Models.UserSettings defaultUserSettings;

            public Handler(UserSettingsRepository repository, Models.UserSettings defaultUserSettings)
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