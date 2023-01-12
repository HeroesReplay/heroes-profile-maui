using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Watchers;

public sealed class BattleLobbySystemWatcher : AbstractGameFileSystemWatcher
{
    public BattleLobbySystemWatcher(AppSettings appSettings)
    {
        Path = appSettings.GameTempDirectory;
        Filter = "replay.server.battlelobby";
        EnableRaisingEvents = false;
        IncludeSubdirectories = true;
    }
}