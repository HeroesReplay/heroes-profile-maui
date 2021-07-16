using HeroesProfile.Core.BackgroundServices;
using HeroesProfile.Core.CQRS.Commands;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.UI.Maui.ViewModels;

using MediatR;

using Microsoft.Maui.Controls;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui
{
    public static class Initializer
    {
        private static List<Task> backgroundTasks = new List<Task>();
        private static CancellationTokenSource TokenSource = new CancellationTokenSource();

        private static IServiceProvider ServiceProvider
        {
            get
            {
                IServiceProvider provider = null;
#if WINDOWS
                provider = Microsoft.Maui.MauiWinUIApplication.Current.Services;
#elif MACCATALYST
                provider = Microsoft.Maui.MauiUIApplicationDelegate.Current.Services;
#endif
                return provider;
            }
        }

        public static void Start()
        {
            ReplayProcessor processor = (ReplayProcessor)ServiceProvider.GetService(typeof(ReplayProcessor));
            FileWatchers watchers = (FileWatchers)ServiceProvider.GetService(typeof(FileWatchers));

            IMediator mediator = (IMediator)ServiceProvider.GetService(typeof(IMediator));
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