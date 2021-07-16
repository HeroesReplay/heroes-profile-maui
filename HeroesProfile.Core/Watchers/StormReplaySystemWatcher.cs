using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Watchers
{
    public sealed class StormReplaySystemWatcher : AbstractGameFileSystemWatcher
    {
        public StormReplaySystemWatcher(AppSettings appSettings)
        {
            this.Path = appSettings.GameDocumentsDirectory;
            this.Filter = "*.StormReplay";
            this.EnableRaisingEvents = false;
            this.IncludeSubdirectories = true;
        }
    }
}