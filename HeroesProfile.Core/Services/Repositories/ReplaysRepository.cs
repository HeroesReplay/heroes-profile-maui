using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Services.Repositories
{
    public class ReplaysRepository
    {
        private readonly string replays = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Heroes Profile", "replays.json");

        private readonly JsonSerializerOptions writeOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        private readonly JsonSerializerOptions readOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
        };

        private readonly SemaphoreSlim semaphore = new(1, 1);

        public async Task<StoredReplay> FindAsync(string path, CancellationToken token)
        {
            var replays = await LoadAsync(token);
            return replays.Find(replay => string.Equals(replay.Path, path, StringComparison.OrdinalIgnoreCase));
        }

        public async Task InsertAsync(StoredReplay replay, CancellationToken token)
        {
            List<StoredReplay> replays = await LoadAsync(token);

            await SaveCollectionAsync(replays.Prepend(replay).Distinct(), token);
        }

        public async Task<StoredReplay> UpdateAsync(StoredReplay replay, CancellationToken token)
        {
            List<StoredReplay> replays = await LoadAsync(token);
            replays.RemoveAll(stored => stored.Path.Equals(replay.Path));
            replays.Insert(0, replay);
            await SaveCollectionAsync(replays.Distinct(), token);
            return replay;
        }

        public async Task InsertAsync(IEnumerable<StoredReplay> replays, CancellationToken token)
        {
            List<StoredReplay> current = await LoadAsync(token);

            await SaveCollectionAsync(replays.Concat(current).Distinct(), token);
        }

        private async Task SaveCollectionAsync(IEnumerable<StoredReplay> replays, CancellationToken token)
        {
            await semaphore.WaitAsync(token);

            await using (FileStream stream = File.OpenWrite(this.replays))
            {
                await JsonSerializer.SerializeAsync(stream, replays, writeOptions, token);
            }

            semaphore.Release();
        }

        public async Task<List<StoredReplay>> LoadAsync(CancellationToken token)
        {
            await semaphore.WaitAsync(token);

            try
            {
                await using (FileStream stream = File.OpenRead(replays))
                {
                    return await JsonSerializer.DeserializeAsync<List<StoredReplay>>(stream, readOptions, token);
                }
            }
            catch (FileNotFoundException ex)
            {
                return new List<StoredReplay>();
            }

            finally
            {
                semaphore.Release();
            }
        }
    }
}
