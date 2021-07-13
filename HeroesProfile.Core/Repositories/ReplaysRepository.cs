using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Repositories
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
            List<StoredReplay> store = await LoadAsync(token);

            return store.Find(replay => string.Equals(replay.Path, path, StringComparison.OrdinalIgnoreCase));
        }

        public async Task InsertAsync(StoredReplay replay, CancellationToken token)
        {
            List<StoredReplay> store = await LoadAsync(token);

            await SaveCollectionAsync(store.Prepend(replay).Distinct(), token);
        }


        public async Task<IEnumerable<StoredReplay>> UpdateAsync(IEnumerable<StoredReplay> replays, CancellationToken token)
        {
            List<StoredReplay> store = await LoadAsync(token);

            foreach (StoredReplay replay in replays)
            {
                store.RemoveAll(stored => stored.Path.Equals(replay.Path));
                store.Insert(0, replay);
            }

            await SaveCollectionAsync(store.Distinct(), token);

            return replays;
        }

        public async Task InsertAsync(IEnumerable<StoredReplay> replays, CancellationToken token)
        {
            List<StoredReplay> current = await LoadAsync(token);

            await SaveCollectionAsync(replays.Concat(current).Distinct(), token);
        }

        private async Task SaveCollectionAsync(IEnumerable<StoredReplay> replays, CancellationToken token)
        {
            await semaphore.WaitAsync(token);

            await using (FileStream stream = new FileInfo(this.replays).Create())
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
            catch (DirectoryNotFoundException ex)
            {
                return new List<StoredReplay>();
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
