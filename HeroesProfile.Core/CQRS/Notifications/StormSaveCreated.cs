using System;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.CQRS.Queries;
using MauiApp2.Core.CQRS.Commands.Twitch;
using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Notifications
{
    public static class StormSaveCreated
    {
        public record Notification(ReplayParseData Data) : INotification;

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
                var settings = await userSettingsRepository.LoadAsync(cancellationToken);

                if (settings.EnableTalentsExtension)
                {
                    await mediator.Send(new UpdateTalents.Command(notification.Data.Replay, notification.Data.ParseType), cancellationToken);
                }

                if (settings.EnableDiscordEnhancement)
                {
                    await mediator.Send(new Commands.Discord.SetActivity.Command(), cancellationToken);
                }
            }
        }
    }
}