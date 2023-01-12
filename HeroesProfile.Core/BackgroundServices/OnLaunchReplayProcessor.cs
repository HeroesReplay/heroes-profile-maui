using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Commands.Replays;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Core.BackgroundServices;

/*
 * This background service is executed at launch to process any replays
 * that would not be picked up by the Replay File Watcher.
 * 
 * Once this service has finished processing unknown replays that are not stored
 * and once it has successfully uploaded all replays that were pending or errored
 * it will exit. 
 * 
 * Once exited. The only other service responsible for ensuring replays are uploaded
 * is the StormReplayWatcher.
 * 
 * Alternatively, we could just keep this background task running and have a very long
 * delay between each cycle, but im not sure about that inside a desktop app.
 */
public class OnLaunchReplayProcessor : BackgroundService
{
    private readonly IMediator mediator;
    private readonly AppSettings appSettings;
    private readonly UserSettingsRepository userSettingsRepository;
    private bool started;

    public OnLaunchReplayProcessor(IMediator mediator, AppSettings appSettings, UserSettingsRepository userSettingsRepository)
    {
        this.mediator = mediator;
        this.appSettings = appSettings;
        this.userSettingsRepository = userSettingsRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (started) return;
        started = true;

        if (!appSettings.EnableReplayProcessing) return;

        bool processOld = true;
        bool complete = false;

        while (!stoppingToken.IsCancellationRequested)
        {
            var userSettings = await userSettingsRepository.LoadAsync(stoppingToken);

            if (processOld)
            {
                /*
                 * BULK UPLOAD OLDEST 10 ITEMS, OLDEST ITEM FIRST
                 * OLDEST FIRST
                */
                ProcessOldestUnknownReplay.Response processResponse = await mediator.Send(new ProcessOldestUnknownReplay.Command(Take: 10), stoppingToken);

                if (processResponse.Processed.Any())
                {
                    foreach (ProcessOldestUnknownReplay.Item item in processResponse.Processed.Where(x => x.ParseData.ParseResult == ParseResult.Success).OrderByDescending(x => x.StoredReplay.Created).Reverse())
                    {
                        await mediator.Send(new UploadAndUpdateReplay.Command(item.StoredReplay), stoppingToken);
                    }
                }
                else
                {
                    // No more old replays detected.
                    processOld = false;
                }
            }

            // We want to ensure we upload every single OLD upload first, before attempting to upload
            // Anything which is just pending (recently added items, we want to ensure we upload as accurate as possible for MMR)

            if (!processOld)
            {
                /* 
                 * CHECK FOR PENDING OR ERROR ITEMS NOT PART OF PREVIOUS 10 (FAILED, OR CANCELLED IN PAST)
                 * BECAUSE NOT LOADED, PASS IN STORED REPLAY ONLY, IT WILL PARSE FILE IN COMMAND
                 */

                List<GetReplays.Filter> filters = new()
                {
                    new(ParseResult.Success, ProcessStatus.Pending),
                    new(ParseResult.Success, ProcessStatus.Error)
                };

                GetReplays.Response replaysResponse = await mediator.Send(new GetReplays.Query(filters));

                // TODO:
                // Keep track of how many cycles where we still have ProcessStatus Error
                // If we cycle a few times and we still have ProcessStatus Error (Maybe a Service is down, so we should just complete anyway)
                if (replaysResponse.Replays.Any())
                {
                    foreach (StoredReplay storedReplay in replaysResponse.Replays.OrderByDescending(replay => replay.Created).Reverse())
                    {
                        await mediator.Send(new UploadAndUpdateReplay.Command(storedReplay), stoppingToken);
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }
}