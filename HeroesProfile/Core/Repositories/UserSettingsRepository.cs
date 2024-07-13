using System.Text.Json;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Repositories;

public class UserSettingsRepository(IPreferences preferences, AppSettings appSettings)
{
    public Task SaveAsync(UserSettings settings, CancellationToken token)
    {
        preferences.Set(nameof(UserSettings.EnablePostMatch), settings.EnablePostMatch);
        preferences.Set(nameof(UserSettings.EnablePreMatch), settings.EnablePreMatch);
        return Task.CompletedTask;
    }

    public async Task<UserSettings> LoadAsync(CancellationToken token)
    {
        return new UserSettings()
        {
            EnablePostMatch = preferences.Get(nameof(UserSettings.EnablePostMatch), true),
            EnablePreMatch = preferences.Get(nameof(UserSettings.EnablePreMatch), true)
        };
    }
}