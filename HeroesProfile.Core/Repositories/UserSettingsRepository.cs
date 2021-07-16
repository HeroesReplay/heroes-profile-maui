using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Repositories
{
    public class UserSettingsRepository
    {
        private readonly AppSettings appSettings;
        private readonly UserSettings defaultSettings;

        private readonly SemaphoreSlim semaphore = new(1, 1);

        private JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true, AllowTrailingCommas = true };

        public UserSettingsRepository(AppSettings appSettings, UserSettings defaultSettings)
        {
            this.appSettings = appSettings;
            this.defaultSettings = defaultSettings;
        }

        public async Task InitilizeAsync(CancellationToken token)
        {
            await semaphore.WaitAsync(token);

            var json = JsonSerializer.Serialize(defaultSettings, options);
            await File.WriteAllTextAsync(appSettings.UserSettingsPath, json, token);

            semaphore.Release();
        }

        public async Task SaveAsync(UserSettings settings, CancellationToken token)
        {
            await semaphore.WaitAsync(token);

            var json = JsonSerializer.Serialize<UserSettings>(settings, options);
            await File.WriteAllTextAsync(appSettings.UserSettingsPath, json, token);

            semaphore.Release();
        }

        public async Task<UserSettings> LoadAsync(CancellationToken token)
        {
            try
            {
                await semaphore.WaitAsync(token);

                var json = await File.ReadAllTextAsync(appSettings.UserSettingsPath, token);
                return JsonSerializer.Deserialize<UserSettings>(json, options);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}