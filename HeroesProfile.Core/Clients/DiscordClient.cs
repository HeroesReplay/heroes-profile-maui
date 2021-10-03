using System;
using System.Collections.Generic;
using System.Linq;

using Heroes.ReplayParser;

using MauiApp2.Core.Models;

using NetDiscordRpc;
using NetDiscordRpc.RPC;

namespace MauiApp2.Core.Clients
{
    /*
     * Probably only works on Windows since the DiscordRPC uses NamedPipes which is a Windows specific service?
     */
    public class DiscordClient : IDisposable
    {
        private readonly AppSettings appSettings;
        private readonly DiscordRPC discord;

        public DiscordClient(AppSettings appSettings)
        {
            this.appSettings = appSettings;
            this.discord = new DiscordRPC(appSettings.DiscordApplicationId, pipe: -1, autoEvents: true, client: null);
        }

        public void UpdatePresence(SessionData sessionData, UserSettings settings, IEnumerable<int> battleNetIds)
        {
            try
            {
                if (!discord.IsInitialized)
                {
                    discord.Initialize();
                }

                // Only update with new presence if state contains battlelobby or stormsave
                discord.ClearPresence();

                if (sessionData.State == SessionState.BattleLobby || sessionData.State == SessionState.StormSave)
                {
                    Player? player = sessionData.Players.FirstOrDefault(p => battleNetIds.Contains(p.BattleNetId));

                    if (player is not null)
                    {
                        int partySize = sessionData.Players.Count(x => x.PartyValue == player.PartyValue && x.Team == player.Team);
                        int partyMax = 5; // All modes are 5 or extract from battlelobby?

                        var presence = new RichPresence
                        {
                            Details = $"{sessionData.Map} ({sessionData.GameMode})",
                            State = $"Playing {player.HeroId}",
                            Assets = new Assets
                            {
                                LargeImageKey = "game",
                                LargeImageText = "Heroes of the Storm",
                                // https://discord.com/developers/applications/892930415738900491/rich-presence/assets
                                SmallImageKey = "abathur", // the keys are the file names updated to the Discord Developer Applications art assets
                                SmallImageText = player.HeroAttributeId
                            },
                            Timestamps = new Timestamps
                            {
                                Start = sessionData.Files?.BattleLobby?.Created
                            },
                            Party = new Party
                            {
                                Size = partySize,
                                Max = partyMax,
                                Privacy = PartyPrivacySettings.Public
                            }
                        };

                        if (settings.EnableDiscordPreMatch && sessionData.PreMatchUri != null)
                        {
                            presence.Buttons = new Button[]
                            {
                                new Button { Label = "Pre Match Details", Url = sessionData.PreMatchUri?.ToString() }
                            };
                        }

                        discord.SetPresence(presence);
                    }
                }

                discord.Invoke();
            }
            catch (Exception e)
            {

            }
        }

        public void Dispose()
        {
            ((IDisposable)discord).Dispose();
        }
    }
}
