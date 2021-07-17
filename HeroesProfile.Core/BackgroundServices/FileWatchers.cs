using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Watchers;

using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.Core.BackgroundServices
{
    public class FileWatchers : BackgroundService
    {
        private readonly ILogger<FileWatchers> logger;
        private readonly IMediator mediator;
        private readonly SessionFileSystemWatcher sessionFileSystemWatcher;
        private readonly IEnumerable<AbstractGameFileSystemWatcher> watchers;

        private readonly TimeSpan waitForUnlock = TimeSpan.FromSeconds(0.500);

        private bool started;

        public FileWatchers(ILogger<FileWatchers> logger, IMediator mediator, IEnumerable<AbstractGameFileSystemWatcher> watchers, SessionFileSystemWatcher sessionFileSystemWatcher)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.sessionFileSystemWatcher = sessionFileSystemWatcher;
            this.watchers = watchers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (started) return;
            started = true;

            await Task.WhenAll(watchers.Select(watcher => Task.Factory.StartNew(() => WaitAndCopy(watcher, stoppingToken), stoppingToken)).Append(Task.Factory.StartNew(() => UpdateAndNotify(sessionFileSystemWatcher, stoppingToken), stoppingToken)).ToList());
        }

        private async Task UpdateAndNotify(SessionFileSystemWatcher watcher, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    WaitForChangedResult waitForChangedResult = watcher.WaitForChanged(WatcherChangeTypes.Created | WatcherChangeTypes.Changed, Timeout.Infinite);

                    string fullName = Path.IsPathFullyQualified(waitForChangedResult.Name) ? waitForChangedResult.Name : Directory.GetFiles(watcher.Path, waitForChangedResult.Name, SearchOption.AllDirectories).First();

                    GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Query(new FileInfo(fullName)), stoppingToken);

                    if (response.Data.ParseResult == ParseResult.Success)
                    {
                        if (response.Data.ParseType == ParseType.BattleLobby)
                        {
                            await mediator.Publish(new BattleLobbyCreated.Notification(response.Data), stoppingToken);
                        }
                        else if (response.Data.ParseType == ParseType.StormSave)
                        {
                            await mediator.Publish(new StormSaveCreated.Notification(response.Data), stoppingToken);
                        }
                        else if (response.Data.ParseType == ParseType.StormReplay)
                        {
                            await mediator.Publish(new StormReplayCreated.Notification(response.Data), stoppingToken);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Error in watcher: {watcher.GetType().Name}");
                }
            }
        }

        private async Task WaitAndCopy(AbstractGameFileSystemWatcher watcher, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                WaitForChangedResult waitForChangedResult = watcher.WaitForChanged(WatcherChangeTypes.Created | WatcherChangeTypes.Changed, Timeout.Infinite);

                if (!string.IsNullOrWhiteSpace(waitForChangedResult.Name))
                {
                    try
                    {
                        string fullName = Path.IsPathFullyQualified(waitForChangedResult.Name) ? waitForChangedResult.Name : Directory.GetFiles(watcher.Path, waitForChangedResult.Name, SearchOption.AllDirectories).First();

                        await Task.Delay(waitForUnlock, stoppingToken);

                        await mediator.Send(new UpdateSessionData.Command(fullName), stoppingToken);
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}