using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.Models;
using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications
{
    public static class StormReplay
    {
        public record Created(ReplayParseData ParseResult) : INotification;

        public class Handler : INotificationHandler<Created>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task Handle(Created notification, CancellationToken cancellationToken)
            {
                await mediator.Send(new UpdateSession.Command(notification.ParseResult.Replay, notification.ParseResult.ParseType));
                await mediator.Send(new SaveReplays.Command(notification.ParseResult), cancellationToken);
            }
        }
    }
}