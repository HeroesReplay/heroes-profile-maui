using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.UI.Maui.ViewModels;

using MediatR;

using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui.Services
{
    public class SessionUpdatedNotificationHandler : INotificationHandler<SessionUpdated.Notification>
    {
        private readonly SessionViewModel sessionViewModel;

        public SessionUpdatedNotificationHandler(SessionViewModel sessionViewModel)
        {
            this.sessionViewModel = sessionViewModel;
        }

        public async Task Handle(SessionUpdated.Notification notification, CancellationToken cancellationToken)
        {
            sessionViewModel.Session = notification.Session;
        }
    }
}
