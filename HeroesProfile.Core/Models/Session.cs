using System.Collections.Generic;
using Heroes.ReplayParser;

namespace HeroesProfile.Core.Models
{
    public class Session
    {
        public SessionState State { get; init; }

        public Replay BattleLobby { get; init; }
        public Replay StormSave { get; init; }
        public Replay StormReplay { get; init; }

        public int TrackerEventIndex { get; init; }
        public bool UpdatedTalents { get; init; }
        public bool UpdatedMode { get; init; }

        public List<string> PlayerFoundTalents { get; init; }

        public string? TwitchExtensionId { get; init; }
        public string? TwitchPredictionId { get; init; }
        public string? TwitchPredictionWinningOutcomeId { get; init; }
    }
}