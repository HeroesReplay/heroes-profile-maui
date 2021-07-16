using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.UI.Maui.ViewModels;

using MediatR;

using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui.Services
{
    public class StoredReplaySavedNotificationHandler : INotificationHandler<StoredReplaysUpdated.Notification>
    {

        private readonly ReplaysViewModel replaysViewModel;

        public StoredReplaySavedNotificationHandler(ReplaysViewModel replaysViewModel)
        {
            this.replaysViewModel = replaysViewModel;
        }

        public async Task Handle(StoredReplaysUpdated.Notification notification, CancellationToken cancellationToken)
        {
            _ = await replaysViewModel.LoadStoredReplays.Execute().ToTask(cancellationToken);
        }
    }
}
