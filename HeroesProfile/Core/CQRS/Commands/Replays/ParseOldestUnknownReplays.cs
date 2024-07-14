using HeroesProfile.UI.Core.CQRS.Queries;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI.Core.CQRS.Commands.Replays;

public static class ParseOldestReplays
{
    public record Item(StoredReplay StoredReplay, ReplayParseData ParseData);

    public record Response(List<Item> Processed);

    public record Command(int Take) : IRequest<Response>;

    public class Handler(ILogger<Handler> logger, AppSettings appSettings, ReplaysRepository repository, IMediator mediator) : IRequestHandler<Command, Response>
    {
        private int batchSize = Math.Max(Environment.ProcessorCount / 4, 1);

        public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
        {
            List<Item> items = new List<Item>();
            List<FileInfo> replays = (await CombineOldestReplaysWithStored(cancellationToken)).Take(command.Take).ToList();
            List<FileInfo[]> batches = replays.Chunk(batchSize).ToList();

            foreach (FileInfo[] batch in batches)
            {
                // You can batch parse them, but you cant batch upload them
                GetParsedReplay.Response[] parsedResponses = await Task.WhenAll(batch
                    .AsParallel()
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .WithCancellation(cancellationToken)
                    .Select(info => mediator.Send(new GetParsedReplay.Query(info), cancellationToken))
                    .ToArray());

                ReplayParseData[] parsedReplays = parsedResponses.Select(x => x.Data).ToArray();
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

        private string[] GetAllReplays()
        {
            return Directory.GetFiles(appSettings.GameDocumentsDirectory, "*.StormReplay", SearchOption.AllDirectories);
        }

        private async Task<List<FileInfo>> CombineOldestReplaysWithStored(CancellationToken ct)
        {
            List<StoredReplay> storedReplays = await repository.LoadAsync(ct);
            string[] replays = GetAllReplays();
            
            return replays
                .Where(replay => IsReplayProcessable(storedReplays, replay))
                .OrderByDescending(x => File.GetCreationTime(x))
                .Reverse()
                .Select(x => new FileInfo(x))
                .ToList();
        }

        private bool IsReplayProcessable(List<StoredReplay> storedReplays, string path)
        {
            // New file completely untracked
            var isUntracked = storedReplays.Find(stored => stored.Path == path) == null;

            // File is pending or errored in a previous run
            var isPending = storedReplays.Find(stored => stored.Path == path && (stored.ProcessStatus == ProcessStatus.Pending || stored.ProcessStatus == ProcessStatus.Error)) != null;
            
            return isUntracked || isPending;
        }
    }
}