using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Clients;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class CreateTalents
    {
        public record Command : IRequest<Response>;

        public record Response(SessionData Session);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly SessionRepository sessionRepository;
            private readonly TalentsClient talentsClient;
            private readonly UserSettingsRepository userSettingsRepository;
            private readonly IMediator mediator;

            public Handler(SessionRepository sessionRepository, TalentsClient talentsClient, UserSettingsRepository userSettingsRepository, IMediator mediator)
            {
                this.sessionRepository = sessionRepository;
                this.talentsClient = talentsClient;
                this.userSettingsRepository = userSettingsRepository;
                this.mediator = mediator;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                UserSettings userSettings = await userSettingsRepository.LoadAsync(cancellationToken);

                var sessionId = await talentsClient.CreateSession(userSettings.TalentsIdentity, cancellationToken);

                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    sessionRepository.Session.TalentsExtension.SessionId = sessionId;
                    await talentsClient.SavePlayerData(userSettings.TalentsIdentity, sessionRepository.Session, cancellationToken);
                    await talentsClient.NotifyTwitchTalentChange(userSettings.TalentsIdentity, cancellationToken);

                    await mediator.Publish(new Notifications.SessionUpdated.Notification(sessionRepository.Session), cancellationToken);
                }


                return new Response(this.sessionRepository.Session);
            }
        }
    }
}