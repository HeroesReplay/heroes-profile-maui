using System;
using System.IO;

namespace HeroesProfile.Core.Models
{
    public class Settings
    {
        public string SafeTempPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "Heroes Profile - Safe");
        public string GameTempPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "Heroes of the Storm");
        public string GameDocumentsPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Heroes of the Storm");
        public string StoredReplaysPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Heroes Profile");
        public string RecordTempPath { get; set; } = "C:\\temp";
        public string HeroesProfileUrl { get; set; } = "https://www.heroesprofile.com/";

        public string HotsAPIApiEndpoint { get; set; }
        public string HeroesProfileApiUrl { get; set; } = "https://api.heroesprofile.com/";
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH-mm-ss";
        public int WatcherBufferSize { get; set; } = 4096 * 2;
        public int PredictionWindowSeconds { get; set; } = 300;
        public int HotsAPIMinBuild { get; set; } = 43905;
        public bool EnableRecord { get; set; } = false;
        public bool EnableFileSimulator { get; set; } = true;
        public bool EnableFakePrediction { get; set; } = true;
        public bool EnableFakeHttp { get; set; } = true;
        public double SecondsBetweenChecks { get; set; } = 10;
    }
}