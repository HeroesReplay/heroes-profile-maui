using System.Collections.Generic;

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
        public string HeroesProfileTwitchNickname { get; set; }
        public string HeroesProfileUserId { get; set; }
        public string TwitchAccessToken { get; set; }
        public string TwitchClientId { get; set; }

        public IEnumerable<string> BattleTags { get; set; }

        public UserSettings()
        {

        }
    }
}