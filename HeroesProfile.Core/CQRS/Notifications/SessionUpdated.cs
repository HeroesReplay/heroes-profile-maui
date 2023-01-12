
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications;

public static class SessionUpdated
{
    public record Notification(SessionData SessionData) : INotification;

    public class Handler : INotificationHandler<Notification>
    {
        private readonly IMediator mediator;

        public Handler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {



        }
    }
}