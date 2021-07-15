using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;

using Polly;

namespace HeroesProfile.Core.Repositories
{
    public class ReplaysRepository
    {
        private readonly AppSettings appSettings;

        private readonly JsonSerializerOptions? writeOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
        };

        private readonly JsonSerializerOptions readOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
        };

        public ReplaysRepository(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        private static readonly ReaderWriterLock readerWriterLock = new ReaderWriterLock();

        public async Task ClearAsync(CancellationToken token)
        {
            readerWriterLock.AcquireWriterLock(1000);
            await File.WriteAllTextAsync(appSettings.StoredReplaysPath, "[]", token);
            readerWriterLock.ReleaseLock();
        }

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
            readerWriterLock.AcquireWriterLock(1000);

            try
            {
                var json = JsonSerializer.Serialize(replays, writeOptions);
                await File.WriteAllTextAsync(appSettings.StoredReplaysPath, json, token);
            }
            finally
            {
                readerWriterLock.ReleaseLock();
            }
        }

        public async Task<List<StoredReplay>> LoadAsync(CancellationToken token)
        {
            readerWriterLock.AcquireReaderLock(1000);

            try
            {
                string? json = await File.ReadAllTextAsync(appSettings.StoredReplaysPath, token);
                return JsonSerializer.Deserialize<List<StoredReplay>>(json, readOptions) ?? new List<StoredReplay>();
            }
            finally
            {
                readerWriterLock.ReleaseLock();
            }
        }
    }
}
