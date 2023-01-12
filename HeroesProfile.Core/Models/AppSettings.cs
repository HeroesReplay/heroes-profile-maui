using System;
using System.IO;

namespace HeroesProfile.Core.Models;

/*
 * Change the settings in appsettings.Development.json or appsettings.Production.json
 * These settings should be constants for the given environment.
 */
public class AppSettings
{
    public string DiscordApplicationId { get; set; }
    public bool Debug { get; set; }
    public Uri HeroesProfileUri { get; set; }
    public Uri HeroesProfileApiUri { get; set; }

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