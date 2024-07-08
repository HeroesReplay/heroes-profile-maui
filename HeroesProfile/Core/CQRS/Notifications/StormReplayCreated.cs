using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Notifications;

public static class StormReplayCreated
{
    public record Notification(ReplayParseData ReplayParseData) : INotification;

    public class Handler : INotificationHandler<Notification>
    {
        private readonly IMediator mediator;
        private readonly UserSettingsRepository userSettingsRepository;

        public Handler(IMediator mediator, UserSettingsRepository userSettingsRepository)
        {
            this.mediator = mediator;
            this.userSettingsRepository = userSettingsRepository;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            UserSettings settings = await userSettingsRepository.LoadAsync(cancellationToken);
        }
    }
}