using HeroesProfile.UI.Core.CQRS.Notifications;
using MediatR;
using HeroesProfile.UI.ViewModels;

namespace HeroesProfile.UI.Services.Handlers;

public class StoredReplaysNotificationHandler(ReplaysViewModel replaysViewModel) : INotificationHandler<StoredReplaysUpdated.Notification>
{
    public Task Handle(StoredReplaysUpdated.Notification notification, CancellationToken cancellationToken)
    {
        return replaysViewModel.LoadAsync(cancellationToken);
    }
}
