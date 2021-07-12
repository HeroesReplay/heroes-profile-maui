using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services;
using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class StormReplayCreated
    {
        public record Notification(ReplayParseData ParseResult) : INotification;

        public class Handler : INotificationHandler<Notification>
        {
            private readonly SessionManager sessionManager;
            private readonly IMediator mediator;

            public Handler(SessionManager sessionManager, IMediator mediator)
            {
                this.sessionManager = sessionManager;
                this.mediator = mediator;
            }

            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                sessionManager.SetStormReplay(notification.ParseResult.Replay);
                await mediator.Send(new SaveReplays.Request(notification.ParseResult), cancellationToken);
            }
        }
    }
}