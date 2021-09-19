using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Maui.ViewModels;

using MediatR;

using System.Threading;
using System.Threading.Tasks;

namespace HeroesProfile.Maui
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
