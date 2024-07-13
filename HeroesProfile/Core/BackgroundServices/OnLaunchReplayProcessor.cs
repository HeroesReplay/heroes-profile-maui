using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.CQRS.Commands.Replays;
using HeroesProfile.UI.Core.CQRS.Queries;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace HeroesProfile.UI.Core.BackgroundServices;

public class OnLaunchReplayProcessor(IMediator mediator, AppSettings appSettings, UserSettingsRepository userSettingsRepository) : BackgroundService
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
                // Find the OLDEST 100 replays and process those first
                ParseOldestUnknownReplays.Response response = await mediator.Send(new ParseOldestUnknownReplays.Command(Take: 100), stoppingToken);

                if (response.Processed.Any())
                {
                    var oldestFirst = response.Processed
                        .Where(x => x.ParseData.ProcessStatus == ProcessStatus.Pending)
                        .OrderBy(x => x.StoredReplay.Created)
                        .ToList();

                    foreach (ParseOldestUnknownReplays.Item item in oldestFirst)
                    {
                        await mediator.Send(new UploadAndUpdateReplay.Command(item.StoredReplay), stoppingToken);
                    }
                }
                else
                {
                    break;
                }
            }

            List<GetReplays.Filter> filters =
            [
                new(ProcessStatus.Pending, StormReplayParseStatus.Unknonwn),
                new(ProcessStatus.Pending, StormReplayParseStatus.Success),
                new(ProcessStatus.Error, StormReplayParseStatus.Success)
            ];

            GetReplays.Response replaysResponse = await mediator.Send(new GetReplays.Query(filters), stoppingToken);

            if (replaysResponse.Replays.Any())
            {
                foreach (StoredReplay storedReplay in replaysResponse.Replays.OrderByDescending(replay => replay.Created).Reverse())
                {
                    await mediator.Send(new UploadAndUpdateReplay.Command(storedReplay), stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}