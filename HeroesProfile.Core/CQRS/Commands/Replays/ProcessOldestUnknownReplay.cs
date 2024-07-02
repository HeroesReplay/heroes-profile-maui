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

        private Heroes.StormReplayParser.ParseOptions options;

        public Handler(AppSettings appSettings, ReplaysRepository repository, IMediator mediator)
        {
            this.appSettings = appSettings;
            this.repository = repository;
            this.mediator = mediator;

            this.options = new Heroes.StormReplayParser.ParseOptions
            {
                AllowPTR = true,
                ShouldParseGameEvents = false,
                ShouldParseMessageEvents = false,
                ShouldParseTrackerEvents = false                
            };
        }

        public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
        {
            List<Item> items = new List<Item>();

            IEnumerable<FileInfo> replays = (await GetOldestUnknownReplays(cancellationToken)).Take(command.Take);

            int batchSize = Math.Max(Environment.ProcessorCount / 4, 1);

            foreach (FileInfo[] batch in replays.Chunk(batchSize).ToList())
            {
                var parseReplayTasks = batch
                    .AsParallel()
                    .WithCancellation(cancellationToken)
                    .Select(info => mediator.Send(new GetParsedReplay.Query(info, options), cancellationToken));

                GetParsedReplay.Response[] parsedResponses = await Task.WhenAll(parseReplayTasks.ToArray());

                // Save batch in one operation (1 read / 1 write)
                var parsedReplays = parsedResponses.Select(x => x.Data).ToArray();
                SaveReplays.Response saveResponse = await mediator.Send(new SaveReplays.Command(parsedReplays), cancellationToken);

                foreach (var parsedReplay in parsedResponses)
                {
                    var replay = saveResponse.StoredReplays.Find(stored => string.Equals(stored.Fingerprint, parsedReplay.Data.Fingerprint, StringComparison.OrdinalIgnoreCase));

                    if (replay is null)
                    {
                        throw new Exception("fingerprint not found");
                    }

                    // Map stored replays to parsed replays
                    var item = new Item(replay, parsedReplay.Data);
                    items.Add(item);
                }
            }

            return new Response(items);
        }

        private IEnumerable<FileInfo> GetAllReplaysOrderedByOldest()
        {
            return new DirectoryInfo(appSettings.GameDocumentsDirectory)
            .EnumerateFiles("*.StormReplay", SearchOption.AllDirectories)
            .OrderBy(x => x.CreationTime);
        }

        private async Task<IEnumerable<FileInfo>> GetOldestUnknownReplays(CancellationToken token)
        {
            List<StoredReplay> storedReplays = await repository.LoadAsync(token);
            IEnumerable<FileInfo> replays = GetAllReplaysOrderedByOldest();
            return replays.Where(replay => storedReplays.Find(stored => stored.Path == replay.FullName) == null).OrderBy(x => x.CreationTime);
        }
    }
}