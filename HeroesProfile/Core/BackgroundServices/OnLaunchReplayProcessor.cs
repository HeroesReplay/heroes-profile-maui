using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.CQRS.Commands.Replays;
using HeroesProfile.UI.Core.CQRS.Queries;
using HeroesProfile.UI.Core.Models;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI.Core.BackgroundServices;

public class OnLaunchReplayProcessor(ILogger<OnLaunchReplayProcessor> logger, IMediator mediator) : BackgroundService
{
    private bool started;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (started) return;
        started = true;

        while (!stoppingToken.IsCancellationRequested)
        {
            while (true)
            {
                ParseOldestReplays.Response response = await mediator.Send(new ParseOldestReplays.Command(Take: 100), stoppingToken);

                if (response.Processed.Any())
                {
                    var oldestFirst = response.Processed
                        .Where(x => x.ParseData.ProcessStatus == ProcessStatus.Pending)
                        .OrderByDescending(x => x.StoredReplay.Created)
                        .Reverse()
                        .ToList();

                    foreach (ParseOldestReplays.Item item in oldestFirst)
                    {
                        await mediator.Send(new UploadAndUpdateReplay.Command(item.StoredReplay), stoppingToken);
                    }
                }
                else
                {
                    logger.LogInformation("No oldest replays to process.");
                    break;
                }
            }

            List<GetReplays.Filter> filters =
            [
                // Process replays that are pending with an unknown parse  (not yet uploaded)
                new(ProcessStatus.Pending, StormReplayParseStatus.Unknonwn),

                // Process replays that are pending with a successful parse (not yet uploaded)
                new(ProcessStatus.Pending, StormReplayParseStatus.Success),

                // Process replays that are errored with a successful parse (upload issues?) 
                new(ProcessStatus.Error, StormReplayParseStatus.Success)
            ];

            GetReplays.Response replaysResponse = await mediator.Send(new GetReplays.Query(filters), stoppingToken);

            if (replaysResponse.Replays.Any())
            {
                foreach (StoredReplay storedReplay in replaysResponse.Replays.OrderByDescending(replay => replay.Created).Reverse())
                {
                    logger.LogInformation("Processing replay {Path} with Status {Status}", storedReplay.Path, storedReplay.ProcessStatus);

                    await mediator.Send(new UploadAndUpdateReplay.Command(storedReplay), stoppingToken);
                }
            }
            else
            {
                logger.LogInformation("No replays to process.");
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}