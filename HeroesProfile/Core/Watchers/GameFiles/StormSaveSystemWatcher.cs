using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Watchers;

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