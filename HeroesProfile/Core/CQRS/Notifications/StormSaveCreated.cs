using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Notifications;

public static class StormSaveCreated
{
    public record Notification(ReplayParseData Data) : INotification;

    public class Handler(IMediator mediator, UserSettingsRepository userSettingsRepository) : INotificationHandler<Notification>
    {
        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}