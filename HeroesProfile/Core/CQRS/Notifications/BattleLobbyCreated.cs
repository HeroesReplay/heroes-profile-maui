using HeroesProfile.UI.Core.CQRS.Commands.Session;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Notifications;

public static class BattleLobbyCreated
{
    public record Notification(ReplayParseData Data) : INotification;

    public class Handler(IMediator mediator, UserSettingsRepository userSettingsRepository) : INotificationHandler<Notification>
    {
        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            var settings = await userSettingsRepository.LoadAsync(cancellationToken);

            if (settings.EnablePreMatch)
            {
                await mediator.Send(new UpdateSessionPreMatch.Command(notification.Data), cancellationToken);
            }
        }
    }
}