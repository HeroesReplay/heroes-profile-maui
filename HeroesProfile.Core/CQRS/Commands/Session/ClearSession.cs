using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands.Session;

public static class ClearSession
{
    public record Command : IRequest<Response>;

    public record Response();

    public class Handler : IRequestHandler<Command, Response>
    {
        private AppSettings appSettings;
        private readonly SessionRepository sessionRepository;
        private readonly IMediator mediator;

        public Handler(AppSettings appSettings, SessionRepository sessionRepository, IMediator mediator)
        {
            this.appSettings = appSettings;
            this.sessionRepository = sessionRepository;
            this.mediator = mediator;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            await sessionRepository.ClearAsync(cancellationToken);
            await mediator.Publish(new SessionUpdated.Notification(sessionRepository.SessionData));

            return new Response();
        }
    }
}