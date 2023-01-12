
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Blazor.ViewModels;

using MediatR;

namespace HeroesProfile.UI.Services.Handlers;

public class SessionNotificationHandler : INotificationHandler<SessionUpdated.Notification>, INotificationHandler<TwitchPredictionUpdated.Notification>, INotificationHandler<TwitchTalentsUpdated.Notification>
{
    private readonly AnalysisViewModel sessionViewModel;

    public SessionNotificationHandler(AnalysisViewModel sessionViewModel)
    {
        this.sessionViewModel = sessionViewModel;
    }

    public Task Handle(SessionUpdated.Notification notification, CancellationToken cancellationToken)
    {
        sessionViewModel.Session = notification.SessionData;
        return Unit.Task;
    }

    public Task Handle(TwitchTalentsUpdated.Notification notification, CancellationToken cancellationToken)
    {
        sessionViewModel.Session = notification.SessionData;
        return Unit.Task;
    }

    public Task Handle(TwitchPredictionUpdated.Notification notification, CancellationToken cancellationToken)
    {
        sessionViewModel.Session = notification.SessionData;
        return Unit.Task;
    }
}
