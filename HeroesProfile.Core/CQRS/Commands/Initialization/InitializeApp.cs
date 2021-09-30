using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.CQRS.Commands.Replays;
using MauiApp2.Core.CQRS.Commands.Session;
using MauiApp2.Core.CQRS.Commands.UserSettings;
using MauiApp2.Core.Models;

using MediatR;

namespace MauiApp2.Core.CQRS.Commands.Initialization
{
    public static class InitializeApp
    {
        public record Command : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly AppSettings appSettings;
            private readonly IMediator mediator;

            public Handler(AppSettings appSettings, IMediator mediator)
            {
                this.appSettings = appSettings;
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
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

                return Unit.Value;
            }
        }
    }
}
