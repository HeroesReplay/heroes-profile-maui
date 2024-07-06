using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Blazor.ViewModels;

using MediatR;
using HeroesProfile.Core.CQRS.Notifications;

namespace HeroesProfile.UI.Services.Handlers;

public class StoredReplaysNotificationHandler(ReplaysViewModel replaysViewModel) : INotificationHandler<StoredReplaysUpdated.Notification>
{
    public Task Handle(StoredReplaysUpdated.Notification notification, CancellationToken cancellationToken)
    {
        return replaysViewModel.LoadAsync(cancellationToken);
    }
}
