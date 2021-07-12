using Microsoft.Maui;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HeroesProfile.UI.Maui.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MiddleApp
    {
        // private readonly CancellationTokenSource Source = new CancellationTokenSource();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            // this.UnhandledException += App_UnhandledException;
        }

        //private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    this.Services.GetRequiredService<ILogger<App>>().LogError(e.Exception, "Unhandled exception");
        //}

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            Microsoft.Maui.Essentials.Platform.OnLaunched(args);
            // this.MainWindow.Closed += MainWindowOnClosed;
        }

        //protected override void OnWindowCreated(WindowCreatedEventArgs args)
        //{
        //    base.OnWindowCreated(args);

        //    //Task.Factory.StartNew(() => this.Services.GetRequiredService<StartupScanner>().StartAsync(Source.Token));
        //    //Task.Factory.StartNew(() => this.Services.GetRequiredService<GameFileWatcher>().StartAsync(Source.Token));
        //}

        //private void MainWindowOnClosed(object sender, WindowEventArgs args)
        //{
        //    Source.Cancel();
        //}
    }

    public class MiddleApp : MauiWinUIApplication<Startup>
    {

    }
}
