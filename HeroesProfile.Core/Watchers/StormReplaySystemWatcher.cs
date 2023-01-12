using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Watchers;

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