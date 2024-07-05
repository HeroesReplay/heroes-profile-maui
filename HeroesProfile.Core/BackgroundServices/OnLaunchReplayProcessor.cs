using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heroes.StormReplayParser;
using HeroesProfile.Core.CQRS.Commands.Replays;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Core.BackgroundServices;

public class OnLaunchReplayProcessor(IMediator mediator, AppSettings appSettings, UserSettingsRepository userSettingsRepository) : BackgroundService
{
    private bool started;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (started) return;
        started = true;

        if (!appSettings.EnableReplayProcessing) return;

        while (!stoppingToken.IsCancellationRequested)
        {
            while (true)
            {
                ParseOldestUnknownReplays.Response response = await mediator.Send(new ParseOldestUnknownReplays.Command(Take: 10), stoppingToken);

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