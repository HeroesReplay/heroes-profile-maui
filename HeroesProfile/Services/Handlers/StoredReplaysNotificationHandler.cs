using HeroesProfile.Blazor.ViewModels;

using MediatR;
using HeroesProfile.Core.CQRS.Notifications;

namespace HeroesProfile.UI.Services.Handlers;

public class StoredReplaysNotificationHandler : INotificationHandler<StoredReplaysUpdated.Notification>
{
    private readonly ReplaysViewModel replaysViewModel;

    public StoredReplaysNotificationHandler(ReplaysViewModel replaysViewModel)
    {
        this.replaysViewModel = replaysViewModel;
    }

    public Task Handle(StoredReplaysUpdated.Notification notification, CancellationToken cancellationToken)
    {
        return replaysViewModel.LoadAsync(cancellationToken);
    }
}
