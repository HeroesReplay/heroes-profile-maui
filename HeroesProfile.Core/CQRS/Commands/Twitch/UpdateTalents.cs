using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heroes.ReplayParser;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class UpdateTalents
    {
        public record Command(Replay Replay, ParseType ParseType) : IRequest<Response>;

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
                Dictionary<string, string> identity = userSettings.TalentsIdentity;

                if (request.ParseType == ParseType.BattleLobby)
                {
                    // BattleLobby is only used for session creation
                }
                else if (request.ParseType == ParseType.StormReplay)
                {
                    await SaveMissingTalents(identity, cancellationToken);
                }
                else if (request.ParseType == ParseType.StormSave)
                {
                    await UpdateSession(identity, sessionRepository.Session, cancellationToken);
                }

                return new Response(sessionRepository.Session);
            }

            private async Task SaveMissingTalents(Dictionary<string, string> identity, CancellationToken cancellationToken)
            {
                await client.SaveMissingTalents(identity, sessionRepository.Session, cancellationToken);
            }

            private async Task UpdateSession(Dictionary<string, string> identity, Session session, CancellationToken cancellationToken)
            {
                if (session.StormSave == null || session.Files.BattleLobby == null || session.BattleLobby == null)
                {
                    return;
                }

                // Can this not be processed once? IsBattleLobbyToStormSaveSynced = false/true
                for (int stormPlayIndex = 0; stormPlayIndex < session.StormSave.Players.Length; stormPlayIndex++)
                {
                    for (int stormLobbyIndex = 0; stormLobbyIndex < session.BattleLobby.Players.Length; stormLobbyIndex++)
                    {
                        if (session.StormSave.Players[stormPlayIndex].Name == session.BattleLobby.Players[stormLobbyIndex].Name)
                        {
                            session.StormSave.Players[stormPlayIndex].BattleTag = session.BattleLobby.Players[stormLobbyIndex].BattleTag;
                            break;
                        }
                    }
                }

                if (session.StormSave.TrackerEvents != null)
                {
                    for (int i = session.Extension.TrackerEventIndex; i < session.StormSave.TrackerEvents.Count; i++)
                    {
                        var trackerEvent = session.StormSave.TrackerEvents[i];
                        string eventName = trackerEvent.Data.dictionary[0].blobText;

                        if (eventName == "TalentChosen")
                        {
                            int playerId = (int)trackerEvent.Data.dictionary[2].optionalData.array[0].dictionary[1].vInt.Value - 1;

                            Player player = session.StormSave.Players[playerId];

                            Talent talent = new Talent()
                            {
                                TalentName = trackerEvent.Data.dictionary[1].optionalData.array[0].dictionary[1].blobText,
                                TimeSpanSelected = trackerEvent.TimeSpan
                            };

                            if (!session.Extension.GameModeUpdated)
                            {
                                await client.UpdateReplayData(identity, session, cancellationToken);
                                await client.UpdatePlayerData(identity, session, cancellationToken);
                                session.Extension.GameModeUpdated = true;
                            }

                            var playerTalent = $"{player.Name}:{talent.TalentName}";

                            if (!session.Extension.PlayerFoundTalents.Contains(playerTalent))
                            {
                                session.Extension.PlayerFoundTalents.Add(playerTalent);
                                await client.SaveTalentData(identity, session, player, talent, cancellationToken);
                                session.Extension.TalentsUpdated = true;
                            }

                            session.Extension.TrackerEventIndex = i;
                        }
                    }

                    if (session.Extension.TalentsUpdated)
                    {
                        await client.NotifyTwitchTalentChange(identity, cancellationToken);
                        session.Extension.TalentsUpdated = false;
                    }
                }

                if (!session.Extension.GameModeUpdated)
                {
                    await client.UpdateReplayData(identity, session, cancellationToken);
                    await client.UpdatePlayerData(identity, session, cancellationToken);
                    session.Extension.GameModeUpdated = true;
                }
            }
        }
    }
}