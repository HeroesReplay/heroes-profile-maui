using System.IO;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Watchers
{
    public sealed class SessionFileSystemWatcher : FileSystemWatcher
    {
        public SessionFileSystemWatcher(AppSettings appSettings)
        {
            this.Path = appSettings.ApplicationSessionDirectory;
            this.Filter = "*.*";
            this.EnableRaisingEvents = false;
            this.IncludeSubdirectories = false;
        }
    }
}