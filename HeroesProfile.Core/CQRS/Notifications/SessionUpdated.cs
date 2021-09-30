
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Notifications
{
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
                var userSettingsResponse = await mediator.Send(new Queries.GetUserSettings.Query(), cancellationToken);

                if (userSettingsResponse.UserSettings.EnableDiscordEnhancement)
                {
                    await mediator.Send(new Commands.Discord.UpdatePresence.Command(), cancellationToken);
                }
            }
        }
    }
}