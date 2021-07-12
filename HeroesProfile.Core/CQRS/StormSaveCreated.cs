using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services;
using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class StormSaveCreated
    {
        public record Notification(ReplayParseData Data) : INotification;

        public class Handler : INotificationHandler<Notification>
        {
            private readonly SessionManager sessionManager;
            private readonly IMediator mediator;

            public Handler(SessionManager sessionManager, IMediator mediator)
            {
                this.sessionManager = sessionManager;
                this.mediator = mediator;
            }

            public Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                sessionManager.SetStormSave(notification.Data.Replay);
                return Task.CompletedTask;
            }
        }
    }
}