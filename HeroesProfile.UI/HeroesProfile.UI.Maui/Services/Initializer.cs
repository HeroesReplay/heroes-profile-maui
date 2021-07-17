using HeroesProfile.Core.BackgroundServices;
using HeroesProfile.Core.CQRS.Commands;

using MediatR;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui
{
    public static class Initializer
    {
        private static List<Task> backgroundTasks = new List<Task>();
        private static CancellationTokenSource TokenSource = new CancellationTokenSource();

        public static void Start()
        {
            ReplayProcessor processor = ServiceProvider.GetService<ReplayProcessor>();
            FileWatchers watchers = ServiceProvider.GetService<FileWatchers>();

            IMediator mediator = ServiceProvider.GetService<IMediator>();

            mediator.Send(new InitializeApp.Command(), TokenSource.Token);

            backgroundTasks.Add(processor.StartAsync(TokenSource.Token));
            backgroundTasks.Add(watchers.StartAsync(TokenSource.Token));
        }

        public static void Stop()
        {
            TokenSource.Cancel();
            Task.WaitAll(backgroundTasks.ToArray());
        }
    }
}