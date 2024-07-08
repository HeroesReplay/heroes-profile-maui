using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Watchers;

public sealed class StormReplaySystemWatcher : AbstractGameFileSystemWatcher
{
    public StormReplaySystemWatcher(AppSettings appSettings)
    {
        Path = appSettings.GameDocumentsDirectory;
        Filter = "*.StormReplay";
        EnableRaisingEvents = false;
        IncludeSubdirectories = true;
    }
}