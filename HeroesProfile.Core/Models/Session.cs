using System.Collections.Generic;

using Heroes.ReplayParser;

namespace HeroesProfile.Core.Models
{
    public class Session
    {
        public SessionState State
        {
            get
            {
                if (StormReplay != null) return SessionState.StormReplay;
                if (StormSave != null) return SessionState.StormSave;
                if (BattleLobby != null) return SessionState.BattleLobby;
                return SessionState.None;
            }
        }

        public SessionFiles Files { get; set; } = new();

        public TwitchPredictionData Prediction { get; set; } = new();

        public TwitchExtensionData Extension { get; set; } = new();

        public Replay? BattleLobby => Files?.BattleLobby;

        public Replay? StormSave => Files?.StormSave;

        public Replay? StormReplay => Files?.StormReplay;

        public Session()
        {
            Files = new SessionFiles();
            Prediction = new TwitchPredictionData();
            Extension = new TwitchExtensionData();
        }
    }

    public class SessionFiles
    {
        public Replay? BattleLobby { get; set; }
        public Replay? StormSave { get; set; }
        public Replay? StormReplay { get; set; }

        public SessionFiles()
        {

        }
    }

    public class TwitchPredictionData
    {
        public string? PredictionId { get; set; }
        public string? WinningOutcomeId { get; set; }
        public string? OtherOutcomeId { get; set; }

        public TwitchPredictionData()
        {

        }
    }

    public class TwitchExtensionData
    {
        public string? SessionId { get; set; }
        public int TrackerEventIndex { get; set; }
        public bool TalentsUpdated { get; set; }
        public bool GameModeUpdated { get; set; }
        public List<string> PlayerFoundTalents { get; set; }

        public TwitchExtensionData()
        {
            PlayerFoundTalents = new List<string>();
        }
    }
}