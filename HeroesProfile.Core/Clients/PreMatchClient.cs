using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Heroes.ReplayParser;

namespace MauiApp2.Core.Clients
{

    public class PreMatchClient
    {
        private readonly HttpClient httpClient;

        public static readonly Uri PreMatchUri = new Uri("PreMatch", UriKind.Relative);

        public PreMatchClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<int?> GetPreMatchId(Replay replay)
        {
            int? preMatchId = null;

            var formData = JsonSerializer.Serialize(replay.Players.Select(player => new { player.Team, player.Name, player.BattleTag, player.BattleNetRegionId }));

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
}
