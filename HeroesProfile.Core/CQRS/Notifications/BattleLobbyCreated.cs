using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.CQRS.Commands.Session;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications;

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