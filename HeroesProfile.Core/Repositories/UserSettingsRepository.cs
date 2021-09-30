using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Models;

namespace MauiApp2.Core.Repositories
{
    public class UserSettingsRepository
    {
        private readonly AppSettings appSettings;

        private readonly SemaphoreSlim semaphore = new(1, 1);

        private JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true, AllowTrailingCommas = true };

        public UserSettingsRepository(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public async Task SaveAsync(UserSettings settings, CancellationToken token)
        {
            await semaphore.WaitAsync(token);

            try
            {
                await File.WriteAllTextAsync(appSettings.UserSettingsPath, JsonSerializer.Serialize(settings, options), token);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<UserSettings> LoadAsync(CancellationToken token)
        {
            try
            {
                await semaphore.WaitAsync(token);
                return JsonSerializer.Deserialize<UserSettings>(await File.ReadAllTextAsync(appSettings.UserSettingsPath, token), options);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}