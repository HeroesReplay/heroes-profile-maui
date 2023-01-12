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

namespace HeroesProfile.Core.CQRS.Commands.Replays;

public static class ProcessOldestUnknownReplay
{
    public record Item(StoredReplay StoredReplay, ReplayParseData ParseData);

    public record Response(IEnumerable<Item> Processed);

    public record Command(int Take) : IRequest<Response>;

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly AppSettings appSettings;
        private readonly ReplaysRepository repository;
        private readonly IMediator mediator;

        public Handler(AppSettings appSettings, ReplaysRepository repository, IMediator mediator)
        {
            this.appSettings = appSettings;
            this.repository = repository;
            this.mediator = mediator;
        }

        public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
        {
            List<Item> items = new List<Item>();

            IEnumerable<FileInfo> replays = (await GetOldestUknownReplays(cancellationToken)).Take(command.Take);

            int batchSize = Math.Max(Environment.ProcessorCount / 4, 1);

            foreach (FileInfo[] batch in replays.Chunk(batchSize).ToList())
            {
                // Parse batch in parallel
                GetParsedReplay.Response[] parsedResponses = await Task.WhenAll(batch.AsParallel().WithCancellation(cancellationToken).Select(info => mediator.Send(new GetParsedReplay.Query(info, null), cancellationToken)).ToArray());

                // Save batch in one operation (1 read / 1 write)
                SaveReplays.Response saveResponse = await mediator.Send(new SaveReplays.Command(parsedResponses.Select(x => x.Data).ToArray()), cancellationToken);

                // Map stored replays to parsed replays
                items.AddRange(parsedResponses.Select(parsed => new Item(saveResponse.StoredReplays.Find(stored => string.Equals(stored.Fingerprint, parsed.Data.Fingerprint, StringComparison.OrdinalIgnoreCase)), parsed.Data)));

                await Task.Delay(2000);
            }

            return new Response(items);
        }

        private IEnumerable<FileInfo> GetAllReplaysOrderedByOldest()
        {
            return new DirectoryInfo(appSettings.GameDocumentsDirectory).EnumerateFiles("*.StormReplay", SearchOption.AllDirectories).OrderBy(x => x.CreationTime);
        }

        private async Task<IEnumerable<FileInfo>> GetOldestUknownReplays(CancellationToken token)
        {
            List<StoredReplay> storedReplays = await repository.LoadAsync(token);
            IEnumerable<FileInfo> replays = GetAllReplaysOrderedByOldest();
            return replays.Where(replay => storedReplays.Find(stored => stored.Path == replay.FullName) == null).OrderBy(x => x.CreationTime);
        }
    }
}