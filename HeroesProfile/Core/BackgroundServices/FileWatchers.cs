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
    private const string BattleLobbyExt = ".battlelobby";
    private const string StormReplayExt = ".StormReplay";
    private const string StormSaveExt = ".StormSave";

    WatcherChangeTypes watcherChangeTypes = WatcherChangeTypes.Created | WatcherChangeTypes.Changed;

    private bool started;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (started) return;
        started = true;

        var watchTasks = watchers.Select(watcher => Task.Factory.StartNew(() => WaitAndCopy(watcher, stoppingToken), stoppingToken)).ToArray();
        var sessionTask = Task.Factory.StartNew(() => UpdateAndNotify(sessionFileSystemWatcher, stoppingToken), stoppingToken);

        await Task.WhenAll(watchTasks.Concat([sessionTask]).ToArray());
    }

    private async Task UpdateAndNotify(SessionFileSystemWatcher watcher, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                WaitForChangedResult result = watcher.WaitForChanged(watcherChangeTypes, Timeout.Infinite);
                if (string.IsNullOrWhiteSpace(result.Name)) continue;

                string fullName = Path.IsPathFullyQualified(result.Name) ? result.Name : Directory.GetFiles(watcher.Path, result.Name, SearchOption.AllDirectories).First();

                GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Query(new FileInfo(fullName)), stoppingToken);

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

    private async Task WaitAndCopy(AbstractGameFileSystemWatcher watcher, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            WaitForChangedResult result = watcher.WaitForChanged(watcherChangeTypes, Timeout.Infinite);
            if (string.IsNullOrWhiteSpace(result.Name)) continue;

            try
            {
                string fullName = Path.IsPathFullyQualified(result.Name) ? result.Name : Directory.GetFiles(watcher.Path, result.Name, SearchOption.AllDirectories).First();

                if (fullName.EndsWith(BattleLobbyExt, StringComparison.OrdinalIgnoreCase))
                {
                    await mediator.Send(new ClearSession.Command(), ct);
                }

                await mediator.Send(new CopyToSession.Command(fullName), ct);

                if (result.Name.EndsWith(StormReplayExt))
                {
                    GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Query(new FileInfo(fullName)), ct);

                    if (response.Data.ParseStatus == StormReplayParseStatus.Success)
                    {
                        SaveReplay.Response saveResponse = await mediator.Send(new SaveReplay.Command(response.Data), ct);
                        UploadAndUpdateReplay.Response uploadAndUpdateResponse = await mediator.Send(new UploadAndUpdateReplay.Command(saveResponse.StoredReplay), ct);

                        if (uploadAndUpdateResponse.Success && uploadAndUpdateResponse.ReplayId.HasValue)
                        {
                            UserSettings settings = await settingsRepository.LoadAsync(ct);

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