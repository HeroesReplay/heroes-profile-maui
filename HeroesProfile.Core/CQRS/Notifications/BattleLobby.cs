using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications
{
    public static class BattleLobby
    {
        public record Created(ReplayParseData Data) : INotification;

        public class Handler : INotificationHandler<Created>
        {
            private readonly IMediator mediator;
            private readonly UserSettingsRepository settingsRepository;

            public Handler(IMediator mediator, UserSettingsRepository settingsRepository)
            {
                this.mediator = mediator;
                this.settingsRepository = settingsRepository;
            }

            public async Task Handle(Created notification, CancellationToken cancellationToken)
            {
                var settings = await settingsRepository.LoadAsync(cancellationToken);

                if (settings.EnableTwitchExtension)
                {
                    await mediator.Send(new CreateTalentsSession.Command(notification.Data.Replay), cancellationToken);
                }

                if (settings.EnablePredictions)
                {
                    await mediator.Send(new CreateTwitchPrediction.Command(), cancellationToken);
                }

                if (settings.EnablePreMatch)
                {
                    // TODO:
                }
            }
        }
    }
}