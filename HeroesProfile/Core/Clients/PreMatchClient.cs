using System.Text.Json;
using Heroes.StormReplayParser;

namespace HeroesProfile.UI.Core.Clients;


public class PreMatchClient(HttpClient httpClient)
{
    public static readonly Uri PreMatchUri = new Uri("PreMatch", UriKind.Relative);

    public async Task<int?> GetPreMatchId(StormReplay replay)
    {
        int? preMatchId = null;

        var formData = JsonSerializer.Serialize(replay.StormPlayers.Select(player => new { player.Team, player.Name, player.BattleTagName, player.ToonHandle!.Id }));

        using (FormUrlEncodedContent content = new(new Dictionary<string, string>() { { "data", formData } }.AsEnumerable()))
        {
            HttpResponseMessage response = await httpClient.PostAsync(PreMatchUri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                if (int.TryParse(result, out var value))
                {
                    preMatchId = value;
                }
            }
        }

        return preMatchId;
    }
}
