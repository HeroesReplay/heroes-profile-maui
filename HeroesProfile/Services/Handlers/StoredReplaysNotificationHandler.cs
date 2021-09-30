
using MauiApp2.Core.CQRS.Notifications;
using MauiApp2.ViewModels;

using MediatR;

using System.Threading;
using System.Threading.Tasks;

namespace MauiApp2.Services.Handlers
{
    public class StoredReplaysNotificationHandler : INotificationHandler<StoredReplaysUpdated.Notification>
    {
        private readonly ReplaysViewModel replaysViewModel;

        public StoredReplaysNotificationHandler(ReplaysViewModel replaysViewModel)
        {
            this.replaysViewModel = replaysViewModel;
        }

        public async Task Handle(StoredReplaysUpdated.Notification notification, CancellationToken cancellationToken)
        {
            await replaysViewModel.LoadAsync(cancellationToken);
        }
    }
}
