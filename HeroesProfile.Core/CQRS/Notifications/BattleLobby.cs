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
            private readonly UserSettingsRepository userSettingsRepository;

            public Handler(IMediator mediator, UserSettingsRepository userSettingsRepository)
            {
                this.mediator = mediator;
                this.userSettingsRepository = userSettingsRepository;
            }

            public async Task Handle(Created notification, CancellationToken cancellationToken)
            {
                var settings = await userSettingsRepository.LoadAsync(cancellationToken);

                if (settings.EnableTwitchExtension)
                {
                    await mediator.Send(new CreateTalents.Command(), cancellationToken);
                }

                if (settings.EnablePredictions)
                {
                    await mediator.Send(new CreatePrediction.Command(), cancellationToken);
                }

                if (settings.EnablePreMatch)
                {
                    // TODO:
                }
            }
        }
    }
}