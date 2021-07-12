using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Core.BackgroundServices
{
    public class FileWatcherService : BackgroundService
    {
        private readonly IMediator mediator;
        private readonly IEnumerable<FileSystemWatcher> watchers;

        private bool started;

        public FileWatcherService(IMediator mediator, IEnumerable<FileSystemWatcher> watchers)
        {
            this.mediator = mediator;
            this.watchers = watchers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (started) return;
            started = true;

            TimeSpan WaitForUnlock = TimeSpan.FromSeconds(2);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Parallel.ForEachAsync(watchers, new ParallelOptions { CancellationToken = stoppingToken, MaxDegreeOfParallelism = watchers.Count() }, async (watcher, token) =>
                {
                    WaitForChangedResult waitForChangedResult = watcher.WaitForChanged(WatcherChangeTypes.Changed | WatcherChangeTypes.Created);

                    if (!string.IsNullOrWhiteSpace(waitForChangedResult.Name))
                    {
                        await Task.Delay(WaitForUnlock, token);

                        GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Query(new FileInfo(waitForChangedResult.Name)), token);

                        if(response.Data.ParseResult == ParseResult.Success)

                        if (response.Data.ParseType == ParseType.BattleLobby)
                        {
                            await mediator.Publish(new BattleLobby.Created(response.Data), token);
                        }
                        else if (response.Data.ParseType == ParseType.StormSave)
                        {
                            await mediator.Publish(new StormSave.Created(response.Data), token);
                        }
                        else if (response.Data.ParseType == ParseType.StormReplay)
                        {
                            await mediator.Publish(new StormReplay.Created(response.Data), token);
                        }
                    }
                });
            }
        }
    }
}