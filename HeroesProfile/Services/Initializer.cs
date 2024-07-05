using HeroesProfile.Core.BackgroundServices;
using HeroesProfile.Core.CQRS.Commands.Initialization;
using MediatR;

namespace HeroesProfile.UI.Services;

public static class Initializer
{
    private static readonly List<Task> BackgroundTasks = new List<Task>();
    private static readonly CancellationTokenSource TokenSource = new CancellationTokenSource();

    public static void Start()
    {
        OnLaunchReplayProcessor processor = ServiceProvider.Current.Services.GetRequiredService<OnLaunchReplayProcessor>();
        FileWatchers watchers = ServiceProvider.Current.Services.GetRequiredService<FileWatchers>();
        IMediator mediator = ServiceProvider.Current.Services.GetRequiredService<IMediator>();
        
        mediator.Send(new InitializeApp.Command(), TokenSource.Token);

        BackgroundTasks.Add(Task.Run(() => processor.StartAsync(TokenSource.Token)));
        BackgroundTasks.Add(Task.Run(() => watchers.StartAsync(TokenSource.Token)));
    }

    public static void Stop()
    {
        TokenSource.Cancel();
        Task.WaitAll(BackgroundTasks.ToArray());
    }
}