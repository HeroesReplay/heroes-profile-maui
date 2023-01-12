using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;
using HeroesProfile.Core.CQRS.Commands.Replays;
using HeroesProfile.Core.CQRS.Commands.Session;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using HeroesProfile.Core.Watchers;

using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.Core.BackgroundServices;

public class FileWatchers : BackgroundService
{
    private readonly ILogger<FileWatchers> logger;
    private readonly IMediator mediator;
    private readonly SessionFileSystemWatcher sessionFileSystemWatcher;
    private readonly UserSettingsRepository settingsRepository;
    private readonly IEnumerable<AbstractGameFileSystemWatcher> watchers;

    private readonly TimeSpan waitForUnlock = TimeSpan.FromSeconds(0.500);

    private bool started;

    public FileWatchers(
    ILogger<FileWatchers> logger,
    IMediator mediator,
    IEnumerable<AbstractGameFileSystemWatcher> watchers,
    SessionFileSystemWatcher sessionFileSystemWatcher,
    UserSettingsRepository settingsRepository)
    {
        this.logger = logger;
        this.mediator = mediator;
        this.sessionFileSystemWatcher = sessionFileSystemWatcher;
        this.settingsRepository = settingsRepository;
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

                /*
                 * FileSystemWatcher is extremely fast. 
                 * The file is almost certainly still locked.
                 * Delay before continuing once the notification is recieved.
                 */
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                string fullName = Path.IsPathFullyQualified(waitForChangedResult.Name) ? waitForChangedResult.Name : Directory.GetFiles(watcher.Path, waitForChangedResult.Name, SearchOption.AllDirectories).First();

                GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Query(new FileInfo(fullName), ParseOptions.DefaultParsing), stoppingToken);

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

            await Task.Delay(waitForUnlock, stoppingToken);

            if (!string.IsNullOrWhiteSpace(waitForChangedResult.Name))
            {
                try
                {
                    string fullName = Path.IsPathFullyQualified(waitForChangedResult.Name) ? waitForChangedResult.Name : Directory.GetFiles(watcher.Path, waitForChangedResult.Name, SearchOption.AllDirectories).First();

                    if (fullName.EndsWith(".battlelobby", StringComparison.OrdinalIgnoreCase))
                    {
                        await mediator.Send(new ClearSession.Command(), stoppingToken);
                    }

                    await mediator.Send(new CopyToSession.Command(fullName), stoppingToken);

                    if (waitForChangedResult.Name.EndsWith(".StormReplay"))
                    {
                        // PARSE
                        GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Query(new FileInfo(fullName), ParseOptions.MinimalParsing), stoppingToken);

                        if (response.Data.ParseResult == ParseResult.Success)
                        {
                            // STORE
                            SaveReplays.Response saveResponse = await mediator.Send(new SaveReplays.Command(response.Data), stoppingToken);
                            StoredReplay storedReplay = saveResponse.StoredReplays.Single();

                            // UPLOAD
                            UploadAndUpdateReplay.Response uploadAndUpdateResponse = await mediator.Send(new UploadAndUpdateReplay.Command(storedReplay), stoppingToken);

                            if (uploadAndUpdateResponse.Success && uploadAndUpdateResponse.ReplayId.HasValue)
                            {
                                UserSettings settings = await settingsRepository.LoadAsync(stoppingToken);

                                if (settings.EnablePostMatch)
                                {
                                    await mediator.Send(new UpdateSessionPostMatch.Command(uploadAndUpdateResponse.ReplayId.Value));
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Error in watcher: {watcher.GetType().Name}");
                }
            }
        }
    }
}