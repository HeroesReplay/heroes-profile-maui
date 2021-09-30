using System.IO;

using MauiApp2.Core.Models;

namespace MauiApp2.Core.Watchers
{
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
}