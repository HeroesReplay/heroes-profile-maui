using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Watchers;

public sealed class SessionFileSystemWatcher : FileSystemWatcher
{
    public SessionFileSystemWatcher(AppSettings appSettings)
    {
        Path = appSettings.ApplicationSessionDirectory;
        Filter = "*.*";
        EnableRaisingEvents = false;
        IncludeSubdirectories = false;
    }
}