using HeroesProfile.UI.Core.CQRS.Notifications;
using HeroesProfile.UI.ViewModels;
using MediatR;

namespace HeroesProfile.UI.Services.Handlers;

public class SessionNotificationHandler(AnalysisViewModel sessionViewModel) : INotificationHandler<SessionUpdated.Notification>
{
    public Task Handle(SessionUpdated.Notification notification, CancellationToken cancellationToken)
    {
        sessionViewModel.Session = notification.SessionData;
        return Task.CompletedTask;
    }
}
