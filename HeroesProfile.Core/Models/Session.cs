using System.Collections.Generic;

using Heroes.ReplayParser;

namespace HeroesProfile.Core.Models
{
    public class Session
    {
        public SessionState State { get; set; }

        // This is the ID returned from Heroes Profile for a twitch extension session
        public string ReplayId { get; set; }

        public Replay BattleLobby { get; set; }
        public Replay StormSave { get; set; }
        public Replay StormReplay { get; set; }

        public int TrackerEventIndex { get; set; }
        public bool TalentsUpdated { get; set; }
        public bool GameModeUpdated { get; set; }

        public List<string> PlayerFoundTalents { get; set; }

        public string? TwitchExtensionId { get; set; }
        public string? TwitchPredictionId { get; set; }
        public string? TwitchPredictionWinningOutcomeId { get; set; }
        public string? TwitchPredictionOtherOutcomeId { get; set; }
    }
}