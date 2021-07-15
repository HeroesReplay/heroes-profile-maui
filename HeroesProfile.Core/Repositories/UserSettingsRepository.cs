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

        public UserSettingsRepository(AppSettings appSettings, UserSettings defaultSettings)
        {
            this.appSettings = appSettings;
            this.defaultSettings = defaultSettings;
        }

        public async Task InitilizeAsync(CancellationToken token)
        {
            await using (var writer = File.OpenWrite(appSettings.UserSettingsPath))
            {
                await JsonSerializer.SerializeAsync(writer, defaultSettings, new JsonSerializerOptions() { WriteIndented = true }, token);
            }
        }

        public async Task SaveAsync(UserSettings settings, CancellationToken token)
        {
            await semaphore.WaitAsync(token);

            using (var stream = File.OpenWrite(appSettings.UserSettingsPath))
            {
                await JsonSerializer.SerializeAsync(stream, settings, new JsonSerializerOptions() { WriteIndented = true }, token);
            }

            semaphore.Release();
        }

        public async Task<UserSettings> LoadAsync(CancellationToken token)
        {
            try
            {
                await semaphore.WaitAsync(token);

                using (var stream = File.OpenRead(appSettings.UserSettingsPath))
                {
                    return await JsonSerializer.DeserializeAsync<UserSettings>(stream, new JsonSerializerOptions() { AllowTrailingCommas = true }, token);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}