using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace HeroesProfile.Core.Models
{
    /*
    * Change the default UserSettings in appsettings.Development.json or appsettings.Production.json
    */
    public class UserSettings
    {
        public bool EnablePostMatch { get; set; }
        public bool EnablePreMatch { get; set; }
        public bool EnablePredictions { get; set; }
        public bool EnableTalentsExtension { get; set; }

        public string? BroadcasterId { get; set; }
        public string? HeroesProfileTwitchKey { get; set; }
        public string? HeroesProfileApiEmail { get; set; }
        public string? HeroesProfileUserId { get; set; }


        [JsonIgnore]
        public Dictionary<string, string> Identity => new()
        {
            { "hp_twitch_key", HeroesProfileTwitchKey ?? "UNSET" },
            { "email", HeroesProfileApiEmail ?? "UNSET" },
            { "twitch_nickname", BroadcasterId ?? "UNSET" },
            { "user_id", HeroesProfileUserId ?? "UNSET" },
        };

        public UserSettings()
        {

        }
    }

    /*
     * Change the settings in appsettings.Development.json or appsettings.Production.json
     * These settings should be constants for the given environment.
     */
    public class AppSettings
    {
        public bool Debug { get; set; }
        public Uri? HeroesProfileUri { get; set; }
        public Uri? HeroesProfileApiUri { get; set; }

        public string GameTempDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "Heroes of the Storm");
        public string GameDocumentsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Heroes of the Storm");
        public string SimulationTargetDirectory => Path.Combine(GameDocumentsDirectory, "Simulation");
        public string ApplicationDataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Heroes Profile");
        public string ApplicationSessionDirectory => Path.Combine(ApplicationDataDirectory, "Session");
        public string SimulationSourceDirectory => Path.Combine(ApplicationDataDirectory, "Simulation");
        public string UserSettingsPath => Path.Combine(ApplicationDataDirectory, "usersettings.json");
        public string StoredReplaysPath => Path.Combine(ApplicationDataDirectory, "replays.json");
        public string DateTimeFormat { get; set; }
        public int PredictionWindowSeconds { get; set; }
        public bool EnableRecord { get; set; }
        public bool EnableReplayProcessing { get; set; }
        public bool EnableFileSimulator { get; set; }
        public bool EnableFakeHttp { get; set; }
        public bool DefaultUserSettingsOnStart { get; set; }
        public bool ClearStoredReplaysOnStart { get; set; }

        public AppSettings()
        {

        }
    }
}