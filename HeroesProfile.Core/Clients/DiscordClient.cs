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
    public class DiscordClient
    {
        private readonly AppSettings appSettings;

        public DiscordClient(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public void UpdatePresence(SessionData sessionData, IEnumerable<int> battleNetIds)
        {
            try
            {
                using (var discord = new DiscordRPC(appSettings.DiscordApplicationId, pipe: -1, autoEvents: true, client: null))
                {
                    discord.Initialize();
                    discord.ClearPresence();

                    // Only update with new presence if state contains battlelobby or stormsave

                    if (sessionData.State == SessionState.BattleLobby || sessionData.State == SessionState.StormSave)
                    {
                        Player player = sessionData.BattleLobby.Players.First(p => battleNetIds.Contains(p.BattleNetId));
                        int partySize = sessionData.BattleLobby.Players.Count(x => x.PartyValue == player.PartyValue);
                        int partyMax = 5; // All modes are 5 or extract from battlelobby?

                        discord.SetPresence(new RichPresence
                        {
                            Details = sessionData.BattleLobby.Map,
                            State = "Playing",
                            Assets = new Assets
                            {
                                LargeImageKey = "game",
                                LargeImageText = "Heroes of the Storm",

                                // https://discord.com/developers/applications/892930415738900491/rich-presence/assets
                                SmallImageKey = player.HeroId, // the keys are the file names updated to the Discord Developer Applications art assets
                                SmallImageText = player.HeroAttributeId
                            },
                            Timestamps = new Timestamps
                            {
                                Start = sessionData.BattleLobby.Timestamp
                            },
                            Buttons = new Button[]
                            {
                                // How does the button work?
                                new Button
                                {
                                    Label = "Pre Match Details",
                                    Url = sessionData.PreMatchUri.ToString(),
                                }
                            },
                            Party = new Party
                            {
                                Size = partySize,
                                Max = partyMax,
                                Privacy = PartyPrivacySettings.Public
                            },
                            Secrets = new Secrets
                            {

                            }
                        });
                    }

                    discord.Invoke();

                    discord.Deinitialize();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
