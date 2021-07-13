using System;
using System.IO;

namespace HeroesProfile.Core.Models
{
    public class Settings
    {
        public bool Debug { get; set; } = true;

        public Uri HeroesProfileUri { get; set; } = new Uri("https://www.heroesprofile.com", UriKind.Absolute);
        public Uri HeroesProfileApiUri { get; set; } = new Uri("https://api.heroesprofile.com", UriKind.Absolute);

        public string GameTempDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "Heroes of the Storm");
        public string GameDocumentsDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Heroes of the Storm");
        public string StoredReplaysDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Heroes Profile");

        public string RecordTempPath { get; set; } = "C:\\temp";
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH-mm-ss";
        public int WatcherBufferSize { get; set; } = 4096 * 2;
        public int PredictionWindowSeconds { get; set; } = 300;
        public bool EnableRecord { get; set; } = false;
        public bool EnableFileSimulator { get; set; } = true;
        public bool EnableFakePrediction { get; set; } = true;
        public bool EnableFakeHttp { get; set; } = true;
    }
}