using HeroesProfile.UI.Core.Models;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Notifications;

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