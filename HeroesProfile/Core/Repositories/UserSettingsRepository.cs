using System.Text.Json;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Repositories;

public class UserSettingsRepository(IPreferences preferences)
{
    public void Save(UserSettings settings)
    {
        preferences.Set(nameof(UserSettings.EnablePostMatch), settings.EnablePostMatch);
        preferences.Set(nameof(UserSettings.EnablePreMatch), settings.EnablePreMatch);
        preferences.Set(nameof(UserSettings.EnableClearTrackedOnStart), settings.EnableClearTrackedOnStart);
        preferences.Set(nameof(UserSettings.EnableFakeUpload), settings.EnableFakeUpload);
        preferences.Set(nameof(UserSettings.EnableMinimizeToTray), settings.EnableMinimizeToTray);
    }

    public UserSettings Load()
    {
        return new UserSettings()
        {
            EnablePostMatch = preferences.Get(nameof(UserSettings.EnablePostMatch), true),
            EnablePreMatch = preferences.Get(nameof(UserSettings.EnablePreMatch), true),
            EnableClearTrackedOnStart = preferences.Get(nameof(UserSettings.EnableClearTrackedOnStart), false),
            EnableMinimizeToTray = preferences.Get(nameof(UserSettings.EnableMinimizeToTray), false),
            EnableFakeUpload = preferences.Get(nameof(UserSettings.EnableFakeUpload), false)
        };
    }
}