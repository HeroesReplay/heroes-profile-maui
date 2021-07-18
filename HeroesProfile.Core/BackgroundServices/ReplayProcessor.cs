using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Core.BackgroundServices
{
    public class ReplayProcessor : BackgroundService
    {
        private readonly IMediator mediator;
        private readonly AppSettings appSettings;
        private readonly UserSettingsRepository userSettingsRepository;
        private bool started;

        public ReplayProcessor(IMediator mediator, AppSettings appSettings, UserSettingsRepository userSettingsRepository)
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

            while (!stoppingToken.IsCancellationRequested)
            {
                var userSettings = await userSettingsRepository.LoadAsync(stoppingToken);

                _ = await mediator.Send(new ProcessOldestUnknownReplay.Command(Take: 10), stoppingToken);

                List<GetReplays.Filter> filters = new()
                {
                    new(ParseResult.Success, ProcessStatus.Pending), // stored and not yet uploaded
                    new(ParseResult.Success, ProcessStatus.Error) // maybe it failed last time
                };

                GetReplays.Response replaysResponse = await mediator.Send(new GetReplays.Query(filters));

                foreach (StoredReplay storedReplay in replaysResponse.Replays.OrderByDescending(replay => replay.Created).Reverse())
                {
                    UploadAndUpdateReplay.Response? response = await mediator.Send(new UploadAndUpdateReplay.Command(storedReplay, HotsApi: false, HotsLogs: false), stoppingToken);

                    if (userSettings.EnablePostMatch && response.Success && response.ReplayId.HasValue)
                    {
                        await mediator.Send(new UpdateSessionPostMatch.Command(storedReplay, response.ReplayId.Value), stoppingToken);
                    }
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}