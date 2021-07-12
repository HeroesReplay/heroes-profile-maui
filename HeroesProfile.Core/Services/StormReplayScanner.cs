using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services.Repositories;

namespace HeroesProfile.Core.Services
{
    public class StormReplayScanner
    {
        private readonly Settings settings;
        private readonly ReplaysRepository replaysRepository;

        public StormReplayScanner(Settings settings, ReplaysRepository replaysRepository)
        {
            this.settings = settings;
            this.replaysRepository = replaysRepository;
        }

        public IEnumerable<FileInfo> GetAllReplays()
        {
            return new DirectoryInfo(settings.GameDocumentsPath).EnumerateFiles("*.StormReplay", SearchOption.AllDirectories);
        }

        public async Task<IEnumerable<FileInfo>> GetNewReplays(CancellationToken token)
        {
            IEnumerable<StoredReplay> loadedReplays = await replaysRepository.LoadAsync(token);
            IEnumerable<FileInfo> replays = GetAllReplays();

            ConcurrentDictionary<string, StoredReplay> storedReplays = new ConcurrentDictionary<string, StoredReplay>(loadedReplays.ToImmutableDictionary(x => x.Path, x => x));

            ConcurrentBag<FileInfo> newReplays = new ConcurrentBag<FileInfo>();

            Parallel.ForEach(replays, (replay) =>
            {
                if (!storedReplays.ContainsKey(replay.FullName))
                {
                    newReplays.Add(replay);
                }
            });

            return newReplays;
        }
    }
}