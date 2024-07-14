using HeroesProfile.UI.Core.CQRS.Commands.Replays;
using HeroesProfile.UI.Core.CQRS.Commands.Session;

using HeroesProfile.UI.Core.Models;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Initialization;

public static class InitializeApp
{
    public record Command : IRequest;

    public class Handler(Models.UserSettings userSettings, IMediator mediator) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            if (userSettings.EnableClearTrackedOnStart)
            {
                await mediator.Send(new ClearStoredReplays.Command(), cancellationToken);
            }

            await mediator.Send(new ClearSession.Command(), cancellationToken);
            await mediator.Send(new InitStoredReplays.Command(), cancellationToken);
        }
    }
}
