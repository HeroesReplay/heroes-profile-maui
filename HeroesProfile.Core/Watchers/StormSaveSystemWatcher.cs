
using MauiApp2.Core.Models;

namespace MauiApp2.Core.Watchers
{
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
}