using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Commands.Session;
using HeroesProfile.Core.CQRS.Commands.Twitch;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications;

public static class BattleLobbyCreated
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
                await mediator.Send(new CreateTalents.Command(), cancellationToken);
            }

            if (settings.EnablePredictions)
            {
                await mediator.Send(new CreatePrediction.Command(), cancellationToken);
            }

            if (settings.EnablePreMatch)
            {
                await mediator.Send(new UpdateSessionPreMatch.Command(notification.Data), cancellationToken);
            }
        }
    }
}