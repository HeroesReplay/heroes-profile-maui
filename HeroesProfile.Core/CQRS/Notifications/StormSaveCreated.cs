using System;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.CQRS.Queries;

using MediatR;
using HeroesProfile.Core.CQRS.Commands.Discord;
using HeroesProfile.Core.CQRS.Commands.Twitch;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

namespace HeroesProfile.Core.CQRS.Notifications;

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
                await mediator.Send(new SetActivity.Command(), cancellationToken);
            }
        }
    }
}