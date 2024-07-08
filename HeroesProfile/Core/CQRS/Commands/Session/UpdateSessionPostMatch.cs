using HeroesProfile.UI.Core.CQRS.Notifications;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Session;

public static class UpdateSessionPostMatch
{
    public record Command(long ReplayId) : IRequest;

    public class Handler : IRequestHandler<Command>
    {
        private readonly SessionRepository sessionRepository;
        private readonly AppSettings appSettings;
        private readonly IMediator mediator;

        private Uri PostMatchUri = new Uri("openApi/Replay/Parsed", UriKind.Relative);

        public Handler(SessionRepository sessionRepository, AppSettings appSettings, IMediator mediator)
        {
            this.sessionRepository = sessionRepository;
            this.appSettings = appSettings;
            this.mediator = mediator;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            if (sessionRepository.SessionData.StormReplay != null)
            {
                sessionRepository.SessionData.PostMatchUri = new Uri(new Uri(appSettings.HeroesProfileApiUri, PostMatchUri), new Uri($"?replayID={request.ReplayId}", UriKind.Relative));
                await mediator.Publish(new SessionUpdated.Notification(sessionRepository.SessionData), cancellationToken);
            }
        }
    }
}