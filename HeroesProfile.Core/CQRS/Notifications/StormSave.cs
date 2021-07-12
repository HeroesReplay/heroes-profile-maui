using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications
{
    public static class StormSave
    {
        public record Created(ReplayParseData Data) : INotification;

        public class Handler : INotificationHandler<Created>
        {
            private readonly SessionRepository sessionManager;
            private readonly IMediator mediator;

            public Handler(SessionRepository sessionManager, IMediator mediator)
            {
                this.sessionManager = sessionManager;
                this.mediator = mediator;
            }

            public async Task Handle(Created notification, CancellationToken cancellationToken)
            {
                await mediator.Send(new UpdateSession.Command(notification.Data.Replay, notification.Data.ParseType));
            }
        }
    }
}