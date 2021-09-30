
using MauiApp2.Core.Models;

namespace MauiApp2.Core.Watchers
{
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
}