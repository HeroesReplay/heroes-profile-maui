using Heroes.StormReplayParser;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;

namespace HeroesProfile.UI.Core.Models;

public record SessionFile(StormReplay Replay, ParseType ParseType, DateTime Created);

public class SessionData
{
    public IEnumerable<StormPlayer> Players => StormReplay?.StormPlayers ?? StormSave?.StormPlayers ?? BattleLobby?.StormPlayers ?? Array.Empty<StormPlayer>();
    public string? Map => StormReplay?.MapInfo.MapName ?? StormSave?.MapInfo.MapName ?? BattleLobby?.MapInfo.MapName ?? "UNKNOWN";
    public DateTime? StartTime => (BattleLobby?.Timestamp ?? StormSave?.Timestamp ?? StormReplay?.Timestamp) ?? DateTime.UtcNow;
    public DateTime? EndTime => StormReplay?.ReplayLength != null ? StartTime.GetValueOrDefault().Add(StormReplay.ReplayLength) : null;
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

    public Uri? PostMatchUri { get; set; }
    public Uri? PreMatchUri { get; set; }

    public ReplayFilesData Files { get; set; } = new();
}

public class ReplayFilesData
{
    public SessionFile? BattleLobby { get; set; }
    public SessionFile? StormSave { get; set; }
    public SessionFile? StormReplay { get; set; }
}