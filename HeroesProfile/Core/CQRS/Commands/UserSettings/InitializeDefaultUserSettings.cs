using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.UserSettings;

public static class InitializeDefaultUserSettings
{
    public record Command : IRequest;

    public class Handler(UserSettingsRepository repository, Models.UserSettings defaultUserSettings) : IRequestHandler<Command>
    {  
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            repository.Save(defaultUserSettings);
        }
    }
}