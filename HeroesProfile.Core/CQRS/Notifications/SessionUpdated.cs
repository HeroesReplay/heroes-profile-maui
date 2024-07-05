
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;

using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications;

public static class SessionUpdated
{
    public record Notification(SessionData SessionData) : INotification;

    public class Handler(IMediator mediator) : INotificationHandler<Notification>
    {
        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {

        }
    }
}