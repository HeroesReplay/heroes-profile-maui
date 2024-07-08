using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.CQRS.Commands.Replays;
using HeroesProfile.UI.Core.CQRS.Commands.Session;
using HeroesProfile.UI.Core.CQRS.Notifications;
using HeroesProfile.UI.Core.CQRS.Queries;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using HeroesProfile.UI.Core.Watchers;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI.Core.BackgroundServices;

public class FileWatchers(
    ILogger<FileWatchers> logger,
    IMediator mediator,
    IEnumerable<AbstractGameFileSystemWatcher> watchers,
    SessionFileSystemWatcher sessionFileSystemWatcher,
    UserSettingsRepository settingsRepository) : BackgroundService
{
    private readonly TimeSpan waitForUnlock = TimeSpan.FromSeconds(1);

    private bool started;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (started) return;
        started = true;

        var watchTasks = watchers.Select(watcher => Task.Factory.StartNew(() => WaitAndCopy(watcher, stoppingToken), stoppingToken)).ToArray();
        var sessionTask = Task.Factory.StartNew(() => UpdateAndNotify(sessionFileSystemWatcher, stoppingToken), stoppingToken);
        await Task.WhenAll(watchTasks.Concat(new[] { sessionTask }).ToArray());
    }

    private async Task UpdateAndNotify(SessionFileSystemWatcher watcher, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                WaitForChangedResult waitForChangedResult = watcher.WaitForChanged(WatcherChangeTypes.Created | WatcherChangeTypes.Changed, Timeout.Infinite);

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                string fullName = Path.IsPathFullyQualified(waitForChangedResult.Name)
                    ? waitForChangedResult.Name
                    : Directory.GetFiles(watcher.Path, waitForChangedResult.Name, SearchOption.AllDirectories).First();

                GetParsedReplay.Response response =
                    await mediator.Send(new GetParsedReplay.Query(new FileInfo(fullName), ParseOptions.DefaultParsing), stoppingToken);

                if (response.Data.Replay != null)
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
                logger.LogError(e, "Error in watcher: {Name}", watcher.GetType().Name);
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
                    string fullName = Path.IsPathFullyQualified(waitForChangedResult.Name)
                        ? waitForChangedResult.Name
                        : Directory.GetFiles(watcher.Path, waitForChangedResult.Name, SearchOption.AllDirectories).First();

                    if (fullName.EndsWith(".battlelobby", StringComparison.OrdinalIgnoreCase))
                    {
                        await mediator.Send(new ClearSession.Command(), stoppingToken);
                    }

                    await mediator.Send(new CopyToSession.Command(fullName), stoppingToken);

                    if (waitForChangedResult.Name.EndsWith(".StormReplay"))
                    {
                        // PARSE
                        var query = new GetParsedReplay.Query(new FileInfo(fullName), ParseOptions.MinimalParsing);
                        GetParsedReplay.Response response = await mediator.Send(query, stoppingToken);

                        if (response.Data.ParseStatus == StormReplayParseStatus.Success)
                        {
                            SaveReplays.Response saveResponse = await mediator.Send(new SaveReplays.Command(response.Data), stoppingToken);
                            StoredReplay storedReplay = saveResponse.StoredReplays.Single();

                            // UPLOAD
                            UploadAndUpdateReplay.Response uploadAndUpdateResponse =
                                await mediator.Send(new UploadAndUpdateReplay.Command(storedReplay), stoppingToken);

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
                    logger.LogError(e, "Error in watcher: {Name}", watcher.GetType().Name);
                }
            }
        }
    }
}