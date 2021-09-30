
using MauiApp2.Core.CQRS.Notifications;

using MauiApp2.ViewModels;

using MediatR;

using System.Threading;
using System.Threading.Tasks;

namespace MauiApp2.Services.Handlers
{
    public class SessionNotificationHandler : INotificationHandler<SessionUpdated.Notification>, INotificationHandler<TwitchPredictionUpdated.Notification>, INotificationHandler<TwitchTalentsUpdated.Notification>
    {
        private readonly AnalysisViewModel sessionViewModel;

        public SessionNotificationHandler(AnalysisViewModel sessionViewModel)
        {
            this.sessionViewModel = sessionViewModel;
        }

        public async Task Handle(SessionUpdated.Notification notification, CancellationToken cancellationToken)
        {
            sessionViewModel.Session = notification.SessionData;
        }

        public async Task Handle(TwitchTalentsUpdated.Notification notification, CancellationToken cancellationToken)
        {
            sessionViewModel.Session = notification.SessionData;
        }

        public async Task Handle(TwitchPredictionUpdated.Notification notification, CancellationToken cancellationToken)
        {
            sessionViewModel.Session = notification.SessionData;
        }
    }
}
