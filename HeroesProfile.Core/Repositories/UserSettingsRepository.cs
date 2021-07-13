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
        private readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Heroes Profile", "settings.json");

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public async Task SaveAsync(UserSettings settings, CancellationToken token)
        {
            await semaphore.WaitAsync(token);

            using (var stream = File.OpenWrite(path))
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

                using (var stream = File.OpenRead(path))
                {
                    return await JsonSerializer.DeserializeAsync<UserSettings>(stream, new JsonSerializerOptions() { AllowTrailingCommas = true }, CancellationToken.None);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}