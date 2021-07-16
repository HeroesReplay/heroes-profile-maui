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
    public static class StormReplayCreated
    {
        public record Notification(ReplayParseData ParseResult) : INotification;

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

                if (settings.EnableTwitchExtension && notification.ParseResult.ParseResult == ParseResult.Success)
                {
                    await mediator.Send(new UpdateTalents.Command(notification.ParseResult.Replay, notification.ParseResult.ParseType), cancellationToken);
                }

                if (settings.EnablePredictions)
                {
                    await mediator.Send(new ClosePrediction.Command(), cancellationToken);
                }

                if (settings.EnablePostMatch)
                {
                    // TODO: OpenPostMatch.Command()
                }


                await mediator.Send(new SaveReplays.Command(notification.ParseResult), cancellationToken);
            }
        }
    }
}