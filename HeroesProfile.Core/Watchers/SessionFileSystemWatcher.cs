using System.IO;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Watchers;

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