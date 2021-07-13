using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroesProfile.Core.Models
{
    public class UserSettings
    {
        public bool EnablePostMatch { get; set; }
        public bool EnablePreMatch { get; set; }
        public bool EnablePredictions { get; set; }
        public bool EnableTwitchExtension { get; set; }
        public string HeroesProfileTwitchKey { get; set; }
        public string HeroesProfileApiEmail { get; set; }
        public string BroadcasterId { get; set; }
        public string HeroesProfileUserId { get; set; }
        public string TwitchAccessToken { get; set; }
        public string TwitchClientId { get; set; }
        public IEnumerable<string> BattleTags { get; set; }
        
        [JsonIgnore]
        public Dictionary<string, string> TalentsIdentity => new()
        {
            { "hp_twitch_key", HeroesProfileTwitchKey },
            { "email", HeroesProfileApiEmail },
            { "twitch_nickname", BroadcasterId },
            { "user_id", HeroesProfileUserId },
        };

        public UserSettings()
        {

        }
    }
}