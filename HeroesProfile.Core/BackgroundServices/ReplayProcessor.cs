using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;

using MediatR;

using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Core.BackgroundServices
{


    public class ReplayProcessor : BackgroundService
    {
        private readonly IMediator mediator;
        private readonly AppSettings appSettings;
        private bool started;

        public ReplayProcessor(IMediator mediator, AppSettings appSettings)
        {
            this.mediator = mediator;
            this.appSettings = appSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (started) return;
            started = true;

            if (!appSettings.EnableReplayProcessing) return;

            TimeSpan TimeBetweenScans = TimeSpan.FromMinutes(10);

            while (!stoppingToken.IsCancellationRequested)
            {
                _ = await mediator.Send(new ProcessAllNewReplays.Command(), stoppingToken);

                List<GetReplays.Filter> filters = new()
                {
                    new(ParseResult.Success, ProcessStatus.Pending), // stored and not yet uploaded
                    new(ParseResult.Success, ProcessStatus.Error) // maybe it failed last time
                };

                GetReplays.Response replaysResponse = await mediator.Send(new GetReplays.Query(filters));

                foreach (StoredReplay storedReplay in replaysResponse.Replays.OrderByDescending(replay => replay.Created).Reverse())
                {
                    _ = await mediator.Send(new UploadAndUpdateReplay.Command(storedReplay, HotsApi: false, HotsLogs: false), stoppingToken);
                }

                await Task.Delay(TimeBetweenScans, stoppingToken);
            }
        }
    }
}