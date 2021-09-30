
using MauiApp2.Core.Models;

namespace MauiApp2.Core.Watchers
{
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
}