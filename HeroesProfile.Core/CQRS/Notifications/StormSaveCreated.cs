using System;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications
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
                var settings = await this.userSettingsRepository.LoadAsync(cancellationToken);

                if (settings.EnableTwitchExtension)
                {
                    await mediator.Send(new UpdateTalents.Command(notification.Data.Replay, notification.Data.ParseType), cancellationToken);
                }
            }
        }
    }
}