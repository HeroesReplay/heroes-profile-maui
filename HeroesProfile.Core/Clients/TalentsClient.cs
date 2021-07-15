using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Clients
{
    public class TalentsClient
    {
        private readonly AppSettings appSettings;
        private readonly HttpClient httpClient;

        public static readonly Uri SaveReplayUri = new("twitch/extension/save/replay", UriKind.Relative);
        public static readonly Uri UpdateReplayDataUri = new("twitch/extension/update/replay/", UriKind.Relative);
        public static readonly Uri SavePlayersUri = new("twitch/extension/save/player", UriKind.Relative);
        public static readonly Uri UpdatePlayerDataUri = new("twitch/extension/update/player", UriKind.Relative);
        public static readonly Uri SaveTalentsUri = new("twitch/extension/save/talent", UriKind.Relative);
        public static readonly Uri NotifyUri = new("twitch/extension/notify/uploader", UriKind.Relative);

        public TalentsClient(AppSettings appSettings, HttpClient httpClient)
        {
            this.appSettings = appSettings;
            this.httpClient = httpClient;
        }

        public async Task<string> CreateSession(Dictionary<string, string> identity, CancellationToken cancellationToken)
        {
            var values = new Dictionary<string, string>(identity)
            {
                { "game_date", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") },
            };

            using (var content = new FormUrlEncodedContent(values))
            {
                HttpResponseMessage response = await httpClient.PostAsync(SaveReplayUri, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    if (int.TryParse(await response.Content.ReadAsStringAsync(cancellationToken), out int value))
                    {
                        return value.ToString();
                    }
                }
            }

            throw new Exception($"Could not create session at {SaveReplayUri}");
        }

        public async Task UpdateReplayData(Dictionary<string, string> identity, Session session, CancellationToken cancellationToken)
        {
            if (session.StormSave != null && session.Extension.SessionId != null)
            {
                Dictionary<string, string> values = new(identity)
                {
                    { "replayID", session.Extension.SessionId },
                    { "game_type", $"{session.StormSave.GameMode}" },
                    { "game_map", session.StormSave.Map },
                    { "game_version", session.StormSave.ReplayVersion },
                    { "region", $"{session.StormSave.Players[0].BattleNetRegionId}" },
                };

                using (var content = new FormUrlEncodedContent(values))
                {
                    HttpResponseMessage response = await httpClient.PostAsync(UpdateReplayDataUri, content, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        // Log error
                    }
                }
            }
            else
            {
                // Could not update
            }
        }

        public async Task SaveTalentData(Dictionary<string, string> identity, Session session, Player player, Talent talent, CancellationToken cancellationToken)
        {
            var values = new Dictionary<string, string>(identity)
            {
                { "replayID", session.Extension.SessionId },
                { "blizz_id", player.BattleNetId.ToString() },
                { "battletag", player.Name + "#" + player.BattleTag },
                { "region", player.BattleNetRegionId.ToString() },
                { "talent", talent.TalentName },
                { "hero", player.Character },
                { "hero_id", player.HeroId },
                { "hero_attribute_id", player.HeroAttributeId },
            };

            using (var content = new FormUrlEncodedContent(values))
            {
                using (var response = await httpClient.PostAsync(SaveTalentsUri, content, cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        // Log error
                    }
                }
            }
        }

        public async Task SavePlayerData(Dictionary<string, string> identity, Session session, CancellationToken cancellationToken)
        {
            foreach (Player player in session.StormSave.Players)
            {
                var values = new Dictionary<string, string>(identity)
                {
                    { "replayID", session.Extension.SessionId },
                    { "battletag", player.Name + "#" +  player.BattleTag },
                    { "team", player.Team.ToString() },
                };

                using (var content = new FormUrlEncodedContent(values))
                {
                    using (HttpResponseMessage response = await httpClient.PostAsync(SavePlayersUri, content, cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            // Log error
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationToken);
            }
        }

        public async Task UpdatePlayerData(Dictionary<string, string> identity, Session session, CancellationToken cancellationToken)
        {
            foreach (Player player in session.StormSave.Players)
            {
                var values = new Dictionary<string, string>(identity)
                {
                    { "replayID", session.Extension.SessionId },
                    { "blizz_id", player.BattleNetId.ToString() },
                    { "battletag", player.Name + "#" + player.BattleTag},
                    { "hero", player.Character },
                    { "hero_id", player.HeroId },
                    { "hero_attribute_id", player.HeroAttributeId },
                    { "team", player.Team.ToString() },
                    { "region", player.BattleNetRegionId.ToString() },
                };

                using (var content = new FormUrlEncodedContent(values))
                {
                    using (var response = await httpClient.PostAsync(UpdatePlayerDataUri, content, cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            // Log error
                        }
                    }
                }
            }
        }

        public async Task NotifyTwitchTalentChange(Dictionary<string, string> identity, CancellationToken cancellationToken)
        {
            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>(identity)))
            {
                using (var response = await httpClient.PostAsync(NotifyUri, content, cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        // Log error
                    }
                }
            }
        }

        public async Task SaveMissingTalents(Dictionary<string, string> identity, Session session, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(session.Extension.SessionId))
            {
                Replay replay = session.StormReplay;

                if (replay != null)
                {
                    foreach (Player player in replay.Players.OrderByDescending(i => i.IsWinner))
                    {
                        if (player.Talents != null)
                        {
                            foreach (Talent talent in player.Talents)
                            {
                                var playerTalent = $"{player.Name}:{talent.TalentName}";

                                if (!session.Extension.PlayerFoundTalents.Contains(playerTalent))
                                {
                                    await SaveTalentData(identity, session, player, talent, cancellationToken);
                                    await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationToken);
                                }
                            }
                        }
                    }

                    await NotifyTwitchTalentChange(identity, cancellationToken);
                }
            }
        }
    }
}
