using HeroesProfile.UI.Core.CQRS.Queries;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI.Core.CQRS.Commands.Replays;

public static class ParseOldestUnknownReplays
{
    public record Item(StoredReplay StoredReplay, ReplayParseData ParseData);

    public record Response(IEnumerable<Item> Processed);

    public record Command(int Take) : IRequest<Response>;

    public class Handler(ILogger<Handler> logger, AppSettings appSettings, ReplaysRepository repository, IMediator mediator) : IRequestHandler<Command, Response>
    {
        private readonly Heroes.StormReplayParser.ParseOptions options = new()
        {
            AllowPTR = true,
            ShouldParseGameEvents = false,
            ShouldParseMessageEvents = false,
            ShouldParseTrackerEvents = false
        };

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

                var parsedReplays = parsedResponses.Select(x => x.Data).ToArray();
                SaveReplays.Response saveResponse = await mediator.Send(new SaveReplays.Command(parsedReplays), cancellationToken);

                foreach (var parsedReplay in parsedResponses)
                {
                    var replay = saveResponse.StoredReplays.Find(stored => string.Equals(stored.Fingerprint, parsedReplay.Data.Fingerprint, StringComparison.OrdinalIgnoreCase));

                    if (replay != null)
                    {
                        items.Add(new Item(replay, parsedReplay.Data));
                    }
                }
            }

            return new Response(items);
        }

        private IEnumerable<FileInfo> GetAllReplaysOrderedByOldest()
        {
            logger.LogInformation("Getting all replays ordered by oldest {GameDocumentsDirectory}", appSettings.GameDocumentsDirectory);

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