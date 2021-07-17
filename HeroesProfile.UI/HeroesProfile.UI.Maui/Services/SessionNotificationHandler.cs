using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.UI.Maui.ViewModels;

using MediatR;

using System.Threading;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui
{
    public class SessionNotificationHandler : INotificationHandler<SessionUpdated.Notification>
    {
        private readonly AnalysisViewModel sessionViewModel;

        public SessionNotificationHandler(AnalysisViewModel sessionViewModel)
        {
            this.sessionViewModel = sessionViewModel;
        }

        public async Task Handle(SessionUpdated.Notification notification, CancellationToken cancellationToken)
        {
            sessionViewModel.Session = notification.Session;
        }
    }
}
