using System.Threading;
using System.Threading.Tasks;

using MediatR;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

namespace HeroesProfile.Core.CQRS.Notifications;

public static class StormSaveCreated
{
    public record Notification(ReplayParseData Data) : INotification;

    public class Handler(IMediator mediator, UserSettingsRepository userSettingsRepository) : INotificationHandler<Notification>
    {
        private readonly IMediator mediator = mediator;
        private readonly UserSettingsRepository userSettingsRepository = userSettingsRepository;

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            
        }
    }
}