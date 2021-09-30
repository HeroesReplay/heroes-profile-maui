using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.CQRS.Commands.Twitch;
using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Notifications
{
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

                if (settings.EnableTalentsExtension && notification.ReplayParseData.ParseResult == ParseResult.Success)
                {
                    await mediator.Send(new UpdateTalents.Command(notification.ReplayParseData.Replay, notification.ReplayParseData.ParseType), cancellationToken);
                }

                if (settings.EnablePredictions)
                {
                    await mediator.Send(new ClosePrediction.Command(), cancellationToken);
                }
            }
        }
    }
}