using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Watchers
{
    public sealed class BattleLobbySystemWatcher : AbstractGameFileSystemWatcher
    {
        public BattleLobbySystemWatcher(AppSettings appSettings)
        {
            this.Path = appSettings.GameTempDirectory;
            this.Filter = "replay.server.battlelobby";
            this.EnableRaisingEvents = false;
            this.IncludeSubdirectories = true;
        }
    }
}