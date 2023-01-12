using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Watchers;

public sealed class StormSaveSystemWatcher : AbstractGameFileSystemWatcher
{
    public StormSaveSystemWatcher(AppSettings appSettings)
    {
        Path = appSettings.GameDocumentsDirectory;
        Filter = "*.StormSave";
        EnableRaisingEvents = false;
        IncludeSubdirectories = true;
    }
}