using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using HeroesProfile.Core.Clients;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class CreateTalentsSession
    {
        public record Command(Replay BattleLobby) : IRequest<Response>;

        public record Response(Session Session);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly SessionRepository sessionRepository;
            private readonly TalentsClient client;
            private readonly UserSettingsRepository userSettingsRepository;

            public Handler(SessionRepository sessionRepository, TalentsClient client, UserSettingsRepository userSettingsRepository)
            {
                this.sessionRepository = sessionRepository;
                this.client = client;
                this.userSettingsRepository = userSettingsRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                UserSettings userSettings = await userSettingsRepository.LoadAsync(cancellationToken);
                string replayId = await client.CreateSession(userSettings.TalentsIdentity, cancellationToken);
                this.sessionRepository.SetBattleLobby(replayId, request.BattleLobby);

                await client.SavePlayerData(userSettings.TalentsIdentity, sessionRepository.GetSession(), cancellationToken);
                await client.NotifyTwitchTalentChange(userSettings.TalentsIdentity, cancellationToken);

                return new Response(this.sessionRepository.GetSession());
            }
        }
    }
}