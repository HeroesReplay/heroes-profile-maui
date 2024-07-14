using System.Text.Json;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Repositories;

public class UserSettingsRepository(IPreferences preferences)
{
    public Task SaveAsync(UserSettings settings, CancellationToken token = default)
    {
        preferences.Set(nameof(UserSettings.EnablePostMatch), settings.EnablePostMatch);
        preferences.Set(nameof(UserSettings.EnablePreMatch), settings.EnablePreMatch);
        preferences.Set(nameof(UserSettings.EnableClearTrackedOnStart), settings.EnableClearTrackedOnStart);
        preferences.Set(nameof(UserSettings.EnableFakeUpload), settings.EnableFakeUpload);
        return Task.CompletedTask;
    }

    public async Task<UserSettings> LoadAsync(CancellationToken token = default)
    {
        return new UserSettings()
        {
            EnablePostMatch = preferences.Get(nameof(UserSettings.EnablePostMatch), true),
            EnablePreMatch = preferences.Get(nameof(UserSettings.EnablePreMatch), true),
            EnableClearTrackedOnStart = preferences.Get(nameof(UserSettings.EnableClearTrackedOnStart), false),
            EnableFakeUpload = preferences.Get(nameof(UserSettings.EnableFakeUpload), false)
        };
    }
}