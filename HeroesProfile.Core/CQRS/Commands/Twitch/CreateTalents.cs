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

        public record Response(Session Session);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly SessionRepository sessionRepository;
            private readonly TalentsClient talentsClient;
            private readonly UserSettingsRepository userSettingsRepository;

            public Handler(SessionRepository sessionRepository, TalentsClient talentsClient, UserSettingsRepository userSettingsRepository)
            {
                this.sessionRepository = sessionRepository;
                this.talentsClient = talentsClient;
                this.userSettingsRepository = userSettingsRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                UserSettings userSettings = await userSettingsRepository.LoadAsync(cancellationToken);
                
                sessionRepository.Session.Extension.SessionId = await talentsClient.CreateSession(userSettings.TalentsIdentity, cancellationToken);

                await talentsClient.SavePlayerData(userSettings.TalentsIdentity, sessionRepository.Session, cancellationToken);
                await talentsClient.NotifyTwitchTalentChange(userSettings.TalentsIdentity, cancellationToken);
                return new Response(this.sessionRepository.Session);
            }
        }
    }
}