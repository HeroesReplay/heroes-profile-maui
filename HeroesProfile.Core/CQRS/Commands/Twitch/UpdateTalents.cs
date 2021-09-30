using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using MauiApp2.Core.Clients;
using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Commands.Twitch
{
    public static class UpdateTalents
    {
        public record Command(Replay Replay, ParseType ParseType) : IRequest<Response>;

        public record Response(SessionData Session);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IMediator mediator;
            private readonly SessionRepository sessionRepository;
            private readonly TalentsClient client;
            private readonly UserSettingsRepository userSettingsRepository;

            public Handler(IMediator mediator, SessionRepository sessionRepository, TalentsClient client, UserSettingsRepository userSettingsRepository)
            {
                this.mediator = mediator;
                this.sessionRepository = sessionRepository;
                this.client = client;
                this.userSettingsRepository = userSettingsRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                Models.UserSettings userSettings = await userSettingsRepository.LoadAsync(cancellationToken);


                if (request.ParseType == ParseType.BattleLobby)
                {
                    // BattleLobby is only used for session creation
                }
                else if (request.ParseType == ParseType.StormSave)
                {
                    await UpdateTwitchTalents(userSettings.Identity, cancellationToken);
                    sessionRepository.SessionData.TalentsExtension.LastUpdate = DateTime.Now;
                    await mediator.Publish(new Notifications.TwitchTalentsUpdated.Notification(sessionRepository.SessionData), cancellationToken);

                }
                else if (request.ParseType == ParseType.StormReplay)
                {
                    await UpdateFinalTwitchTalents(userSettings.Identity, cancellationToken);
                    sessionRepository.SessionData.TalentsExtension.LastUpdate = DateTime.Now;
                    await mediator.Publish(new Notifications.TwitchTalentsUpdated.Notification(sessionRepository.SessionData), cancellationToken);
                }

                return new Response(sessionRepository.SessionData);
            }

            private async Task UpdateFinalTwitchTalents(Dictionary<string, string> identity, CancellationToken cancellationToken)
            {
                await client.SaveMissingTalents(identity, sessionRepository.SessionData, cancellationToken);
            }

            private async Task UpdateTwitchTalents(Dictionary<string, string> identity, CancellationToken cancellationToken)
            {
                SessionData session = sessionRepository.SessionData;

                // We need BOTH BattleLobby data and StormSave data
                if (session.StormSave == null || session.BattleLobby == null) return;

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
                    for (int i = session.TalentsExtension.TrackerEventIndex; i < session.StormSave.TrackerEvents.Count; i++)
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

                            if (!session.TalentsExtension.GameModeUpdated)
                            {
                                await client.UpdateReplayData(identity, session, cancellationToken);
                                await client.UpdatePlayerData(identity, session, cancellationToken);
                                session.TalentsExtension.GameModeUpdated = true;
                            }

                            var playerTalent = $"{player.Name}:{talent.TalentName}";

                            if (!session.TalentsExtension.PlayerFoundTalents.Contains(playerTalent))
                            {
                                session.TalentsExtension.PlayerFoundTalents.Add(playerTalent);
                                await client.SaveTalentData(identity, session, player, talent, cancellationToken);
                                session.TalentsExtension.TalentsUpdated = true;
                            }

                            session.TalentsExtension.TrackerEventIndex = i;
                        }
                    }

                    if (session.TalentsExtension.TalentsUpdated)
                    {
                        await client.NotifyTwitchTalentChange(identity, cancellationToken);
                        session.TalentsExtension.TalentsUpdated = false;
                    }
                }

                if (!session.TalentsExtension.GameModeUpdated)
                {
                    await client.UpdateReplayData(identity, session, cancellationToken);
                    await client.UpdatePlayerData(identity, session, cancellationToken);
                    session.TalentsExtension.GameModeUpdated = true;
                }
            }
        }
    }
}