using System;
using System.Collections.Generic;
using System.Linq;

using Heroes.ReplayParser;

namespace MauiApp2.Core.Models
{
    public class SessionData
    {
        public Player[] Players => StormReplay?.Players ?? StormSave?.Players ?? BattleLobby?.Players ?? Array.Empty<Player>();
        public string Map => StormReplay?.Map ?? StormSave?.Map ?? BattleLobby?.Map ?? "UNKNOWN";
        public DateTime StartTime => (BattleLobby?.Timestamp ?? StormSave?.Timestamp ?? StormReplay?.Timestamp) ?? DateTime.UtcNow;
        public DateTime? EndTime => StormReplay?.ReplayLength != null ? StartTime.Add(StormReplay.ReplayLength) : null;

        public GameMode GameMode => StormReplay?.GameMode ?? StormSave?.GameMode ?? BattleLobby?.GameMode ?? GameMode.Unknown;


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

        public Replay BattleLobby => Files?.BattleLobby?.Replay;
        public Replay StormSave => Files?.StormSave?.Replay;
        public Replay StormReplay => Files?.StormReplay?.Replay;

        public Uri PostMatchUri { get; set; }

        public Uri PreMatchUri { get; set; }

        public ReplayFilesData Files { get; set; }

        public TwitchPredictionData Prediction { get; set; }

        public TwitchTalentsData TalentsExtension { get; set; }

        public SessionData()
        {
            Files = new ReplayFilesData();
            Prediction = new TwitchPredictionData();
            TalentsExtension = new TwitchTalentsData();
        }
    }

    public class ReplayFilesData
    {
        public SessionFile? BattleLobby { get; set; }
        public SessionFile? StormSave { get; set; }
        public SessionFile? StormReplay { get; set; }
    }

    public record SessionFile(Replay Replay, ParseType ParseType, DateTime Created);

    public class TwitchPredictionData
    {
        public string PredictionId { get; set; }
        public string WinningOutcomeId { get; set; }
        public string OtherOutcomeId { get; set; }
        public DateTime? LastUpdate { get; set; }

        public TwitchPredictionData()
        {

        }
    }

    public class TwitchTalentsData
    {
        public string SessionId { get; set; }
        public int TrackerEventIndex { get; set; }
        public bool TalentsUpdated { get; set; }
        public bool GameModeUpdated { get; set; }
        public List<string> PlayerFoundTalents { get; set; }
        public DateTime? LastUpdate { get; set; }

        public TwitchTalentsData()
        {
            PlayerFoundTalents = new List<string>();
        }
    }
}