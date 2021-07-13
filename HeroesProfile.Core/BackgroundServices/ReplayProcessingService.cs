using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Core.BackgroundServices
{
    public class ReplayProcessingService : BackgroundService
    {
        private readonly IMediator mediator;
        private readonly Settings settings;
        private bool started;

        public ReplayProcessingService(IMediator mediator, Settings settings)
        {
            this.mediator = mediator;
            this.settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (started) return;
            started = true;

            TimeSpan TimeBetweenScans = TimeSpan.FromMinutes(10);

            while (!stoppingToken.IsCancellationRequested)
            {
                await mediator.Send(new ProcessAndSaveNewReplays.Command(), stoppingToken);

                List<GetReplays.Filter> filters = new()
                {
                    new(ParseResult.Success, ProcessStatus.Pending), // stored and not yet uploaded
                    new(ParseResult.Success, ProcessStatus.Error) // maybe it failed last time
                };

                GetReplays.Response replaysResponse = await mediator.Send(new GetReplays.Query(filters));

                foreach (StoredReplay storedReplay in replaysResponse.Replays)
                {
                    _ = await mediator.Send(new UploadAndUpdateReplay.Command(storedReplay, HotsApi: false, HotsLogs: false), stoppingToken);
                }

                await Task.Delay(TimeBetweenScans, stoppingToken);
            }
        }
    }
}