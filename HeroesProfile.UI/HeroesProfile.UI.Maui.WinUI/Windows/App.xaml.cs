using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.UI.Xaml;
using HeroesProfile.Core.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HeroesProfile.UI.Maui.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MiddleApp
    {
        private readonly CancellationTokenSource source = new();

        private Task[] backgroundTasks;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.Services.GetRequiredService<ILogger<App>>().LogError(e.Exception, "Unhandled exception");
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            Microsoft.Maui.Essentials.Platform.OnLaunched(args);
            this.MainWindow.Closed += MainWindowOnClosed;

            backgroundTasks = new Task[]
            {
                Task.Factory.StartNew(() => this.Services.GetRequiredService<FileWatchers>().StartAsync(source.Token)),
                Task.Factory.StartNew(() => this.Services.GetRequiredService<ReplayProcessor>().StartAsync(source.Token))
            };
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
        }

        private void MainWindowOnClosed(object sender, WindowEventArgs args)
        {
            source.Cancel();
            Task.WaitAll(backgroundTasks);
        }
    }

    public class MiddleApp : MauiWinUIApplication<Startup>
    {

    }
}
