
using MauiApp2.Core.Clients;
using MauiApp2.Core.CQRS.Notifications;
using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MauiApp2.Core.CQRS.Commands.Session
{
    public static class UpdateSessionPreMatch
    {
        public record Command(ReplayParseData ReplayParseData) : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly PreMatchClient preMatchClient;
            private readonly SessionRepository sessionRepository;
            private readonly AppSettings appSettings;
            private readonly IMediator mediator;
            private readonly Uri PreMatchResultsUri = new Uri("PreMatch/Results", UriKind.Relative);

            public Handler(PreMatchClient preMatchClient, SessionRepository sessionRepository, AppSettings appSettings, IMediator mediator)
            {
                this.preMatchClient = preMatchClient;
                this.sessionRepository = sessionRepository;
                this.appSettings = appSettings;
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                int? response = await preMatchClient.GetPreMatchId(request.ReplayParseData.Replay);

                if (response.HasValue)
                {
                    sessionRepository.SessionData.PreMatchUri = new Uri(new Uri(appSettings.HeroesProfileUri, PreMatchResultsUri), new Uri($"?prematchID={response.Value}", UriKind.Relative));
                    await mediator.Publish(new SessionUpdated.Notification(sessionRepository.SessionData), cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}