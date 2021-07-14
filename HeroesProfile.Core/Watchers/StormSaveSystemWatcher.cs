using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Watchers
{
    public sealed class StormSaveSystemWatcher : AbstractGameFileSystemWatcher
    {
        public StormSaveSystemWatcher(AppSettings appSettings)
        {
            this.Path = appSettings.GameDocumentsDirectory;
            this.Filter = "*.StormSave";
            this.EnableRaisingEvents = false;
            this.IncludeSubdirectories = true;
        }
    }
}