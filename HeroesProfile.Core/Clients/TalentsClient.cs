using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;
using Heroes.StormReplayParser;
using Heroes.StormReplayParser.Player;

using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Clients;

public class TalentsClient
{
    private readonly AppSettings appSettings;
    private readonly HttpClient httpClient;

    public static readonly Uri ValidateUri = new("twitch/validate/heroesprofile/token", UriKind.Relative);
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

    public async Task<(string Error, string UserId)> GetUserIdByAuth(string email, string twitchBroadcasterId, string twitchKey, CancellationToken cancellationToken)
    {
        var authParameters = new Uri($"?email={email}&twitch_nickname={twitchBroadcasterId}&hp_twitch_key={twitchKey}", UriKind.Relative);
        HttpResponseMessage response = await httpClient.GetAsync(new Uri(new Uri(httpClient.BaseAddress, ValidateUri), authParameters), cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (int.TryParse(content, out int userId))
            {
                return (null, $"{userId}");
            }
            else
            {
                return (content, null);
            }
        }

        return ($"HTTP ERROR: {response.StatusCode}", null);
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

    public async Task UpdateReplayData(Dictionary<string, string> identity, SessionData session, CancellationToken cancellationToken)
    {
        if (session.StormSave != null && session.TalentsExtension.SessionId != null)
        {
            Dictionary<string, string> values = new(identity)
            {
                { "replayID", session.TalentsExtension.SessionId },
                { "game_type", $"{session.StormSave.GameMode}" },
                { "game_map", session.StormSave.MapInfo.MapName },
                { "game_version", session.StormSave.ReplayVersion.ToString() },
                { "region", $"{session.StormSave.StormPlayers.Select(sp => sp.ToonHandle?.Region).First() }" },
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

    public async Task SaveTalentData(Dictionary<string, string> identity, SessionData session, Heroes.StormReplayParser.Player.StormPlayer player, Heroes.StormReplayParser.Player.HeroTalent talent, CancellationToken cancellationToken)
    {
        var values = new Dictionary<string, string>(identity)
        {
            { "replayID", session.TalentsExtension.SessionId },
            { "blizz_id", player.ToonHandle.Id.ToString() },
            { "battletag", player.BattleTagName  },
            { "region", player.ToonHandle.Region.ToString() },
            { "talent", talent.TalentNameId },
            { "hero", player.PlayerHero.HeroName },
            { "hero_id", player.PlayerHero.HeroId },
            { "hero_attribute_id", player.PlayerHero.HeroAttributeId },
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

    public async Task SavePlayerData(Dictionary<string, string> identity, SessionData session, CancellationToken cancellationToken)
    {
        // TODO: Serialize and POST as ARRAY of data
        foreach (StormPlayer player in session.BattleLobby.StormPlayers)
        {
            var values = new Dictionary<string, string>(identity)
            {
                { "replayID", session.TalentsExtension.SessionId },
                { "battletag", player.Name + "#" +  player.BattleTagName },
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

    public async Task UpdatePlayerData(Dictionary<string, string> identity, SessionData session, CancellationToken cancellationToken)
    {
        // TODO: Serialize and POST as ARRAY of data
        foreach (StormPlayer player in session.StormSave.StormPlayers)
        {
            var values = new Dictionary<string, string>(identity)
            {
                { "replayID", session.TalentsExtension.SessionId },
                { "blizz_id", player.ToonHandle.Id.ToString() },
                { "battletag", player.Name + "#" + player.BattleTagName},
                { "hero", player.PlayerHero.HeroName },
                { "hero_id", player.PlayerHero.HeroId},
                { "hero_attribute_id", player.PlayerHero.HeroAttributeId },
                { "team", player.Team.ToString() },
                { "region", player.ToonHandle.Region.ToString() },
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

    public async Task SaveMissingTalents(Dictionary<string, string> identity, SessionData session, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(session.TalentsExtension.SessionId))
        {
            StormReplay replay = session.StormReplay;

            if (replay != null)
            {
                // TODO: Serialize and POST as ARRAY of data
                foreach (StormPlayer player in replay.StormPlayers.OrderByDescending(i => i.IsWinner))
                {
                    if (player.Talents != null)
                    {
                        foreach (HeroTalent talent in player.Talents)
                        {
                            var playerTalent = $"{player.Name}:{talent.TalentNameId}";

                            if (!session.TalentsExtension.PlayerFoundTalents.Contains(playerTalent))
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
