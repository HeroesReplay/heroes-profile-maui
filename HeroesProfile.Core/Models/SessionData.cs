using System;
using System.Collections.Generic;
using System.Linq;

using Heroes.ReplayParser;
using Heroes.StormReplayParser;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;

namespace HeroesProfile.Core.Models;

public class SessionData
{
    public IEnumerable<StormPlayer> Players => StormReplay?.StormPlayers ?? StormSave?.StormPlayers ?? BattleLobby?.StormPlayers ?? Array.Empty<StormPlayer>();
    public string Map => StormReplay?.MapInfo.MapName ?? StormSave?.MapInfo.MapName ?? BattleLobby?.MapInfo.MapName ?? "UNKNOWN";
    public DateTime StartTime => (BattleLobby?.Timestamp ?? StormSave?.Timestamp ?? StormReplay?.Timestamp) ?? DateTime.UtcNow;
    public DateTime? EndTime => StormReplay?.ReplayLength != null ? StartTime.Add(StormReplay.ReplayLength) : null;
    public StormGameMode GameMode => StormReplay?.GameMode ?? StormSave?.GameMode ?? BattleLobby?.GameMode ?? StormGameMode.Unknown;


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

    public StormReplay? BattleLobby => Files?.BattleLobby?.Replay;
    public StormReplay? StormSave => Files?.StormSave?.Replay;
    public StormReplay? StormReplay => Files?.StormReplay?.Replay;

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

public record SessionFile(StormReplay Replay, ParseType ParseType, DateTime Created);

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