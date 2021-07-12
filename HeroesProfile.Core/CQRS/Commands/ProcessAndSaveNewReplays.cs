using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class ProcessAndSaveNewReplays
    {
        public record Item(StoredReplay StoredReplay, ReplayParseData ParseData);

        public record Response(IEnumerable<Item> Processed);

        public record Command : IRequest<Response>;

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly Settings settings;
            private readonly ReplaysRepository repository;
            private readonly IMediator mediator;

            public Handler(Settings settings, ReplaysRepository repository, IMediator mediator)
            {
                this.settings = settings;
                this.repository = repository;
                this.mediator = mediator;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                List<Item> items = new List<Item>();

                IEnumerable<FileInfo> replays = await GetNewReplaysAsync(cancellationToken);

                int batchSize = Math.Max(Environment.ProcessorCount / 4, 1);

                foreach (FileInfo[] batch in replays.Chunk(batchSize).ToList())
                {
                    // Parse batch in parallel
                    GetParsedReplay.Response[] parsedResponses = await Task.WhenAll(batch.AsParallel().WithCancellation(cancellationToken).Select(info => mediator.Send(new GetParsedReplay.Query(info), cancellationToken)).ToArray());

                    // Save batch in one operation (1 read / 1 write)
                    SaveReplays.Response saveResponse = await mediator.Send(new SaveReplays.Command(parsedResponses.Select(x => x.Data).ToArray()), cancellationToken);

                    // Map stored replays to parsed replays
                    items.AddRange(parsedResponses.Select(parsed => new Item(saveResponse.StoredReplays.Find(stored => string.Equals(stored.Fingerprint, parsed.Data.Fingerprint, StringComparison.OrdinalIgnoreCase)), parsed.Data)));
                }

                return new Response(items);
            }

            private IEnumerable<FileInfo> GetAllReplays()
            {
                return new DirectoryInfo(settings.GameDocumentsPath).EnumerateFiles("*.StormReplay", SearchOption.AllDirectories);
            }

            private async Task<IEnumerable<FileInfo>> GetNewReplaysAsync(CancellationToken token)
            {
                IEnumerable<StoredReplay> loadedReplays = await repository.LoadAsync(token);
                IEnumerable<FileInfo> replays = GetAllReplays();

                ConcurrentDictionary<string, StoredReplay> storedReplays = new ConcurrentDictionary<string, StoredReplay>(loadedReplays.ToDictionary(x => x.Path, x => x));

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
}