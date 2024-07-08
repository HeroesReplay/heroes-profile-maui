using HeroesProfile.UI.Core.Clients;
using HeroesProfile.UI.Core.CQRS.Notifications;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Session;

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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            int? response = await preMatchClient.GetPreMatchId(request.ReplayParseData.Replay);

            if (response.HasValue)
            {
                sessionRepository.SessionData.PreMatchUri = new Uri(new Uri(appSettings.HeroesProfileUri, PreMatchResultsUri), new Uri($"?prematchID={response.Value}", UriKind.Relative));
                await mediator.Publish(new SessionUpdated.Notification(sessionRepository.SessionData), cancellationToken);
            }
        }
    }
}