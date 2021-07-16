using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
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
                    await mediator.Send(new InitializeUserSettings.Command(), cancellationToken);
                }


                return Unit.Value;
            }
        }
    }
}
