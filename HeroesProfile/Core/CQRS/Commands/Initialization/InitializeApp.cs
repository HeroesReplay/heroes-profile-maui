using HeroesProfile.UI.Core.CQRS.Commands.Replays;
using HeroesProfile.UI.Core.CQRS.Commands.Session;
using HeroesProfile.UI.Core.CQRS.Commands.UserSettings;
using HeroesProfile.UI.Core.Models;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Initialization;

public static class InitializeApp
{
    public record Command : IRequest;

    public class Handler(AppSettings appSettings, IMediator mediator) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            if (appSettings.ClearStoredReplaysOnStart)
            {
                await mediator.Send(new ClearStoredReplays.Command(), cancellationToken);
            }

            if (appSettings.DefaultUserSettingsOnStart)
            {
                await mediator.Send(new InitializeDefaultUserSettings.Command(), cancellationToken);
            }

            await mediator.Send(new ClearSession.Command(), cancellationToken);
            await mediator.Send(new InitStoredReplays.Command(), cancellationToken);
        }
    }
}
