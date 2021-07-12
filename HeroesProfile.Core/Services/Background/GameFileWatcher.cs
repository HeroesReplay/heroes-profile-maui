using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS;
using HeroesProfile.Core.Models;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Core.Services.Background
{
    public class GameFileWatcher : BackgroundService
    {
        private readonly IMediator mediator;
        private readonly IEnumerable<FileSystemWatcher> watchers;

        private bool started;

        public GameFileWatcher(IMediator mediator, IEnumerable<FileSystemWatcher> watchers)
        {
            this.mediator = mediator;
            this.watchers = watchers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (started) return;
            started = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Parallel.ForEachAsync(watchers, stoppingToken, async (watcher, token) =>
                {
                    WaitForChangedResult waitForChangedResult = watcher.WaitForChanged(WatcherChangeTypes.Changed | WatcherChangeTypes.Created);

                    await Task.Delay(TimeSpan.FromSeconds(2), token);

                    if (!string.IsNullOrWhiteSpace(waitForChangedResult.Name))
                    {
                        GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Request(new FileInfo(waitForChangedResult.Name)), token);

                        if (response.Data.ParseType == ParseType.BattleLobby)
                        {
                            await mediator.Publish(new BattleLobbyCreated.Notification(response.Data), token);
                        }
                        else if (response.Data.ParseType == ParseType.StormSave)
                        {
                            await mediator.Publish(new StormSaveCreated.Notification(response.Data), token);
                        }
                        else if (response.Data.ParseType == ParseType.StormReplay)
                        {
                            await mediator.Publish(new StormReplayCreated.Notification(response.Data), token);
                        }
                    }
                });
            }
        }
    }
}