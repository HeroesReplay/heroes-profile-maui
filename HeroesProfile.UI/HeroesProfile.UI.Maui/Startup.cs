
using HeroesProfile.Core;

using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace HeroesProfile.UI.Maui
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .RegisterBlazorMauiWebView(typeof(Startup).Assembly)
                .UseMicrosoftExtensionsServiceProviderFactory()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .EnableHotReload()
                .ConfigureHostConfiguration(configBuilder =>
                {

                })
                .ConfigureLifecycleEvents((context, lifecycleBuilder) =>
                {
#if WINDOWS
                    //lifecycle
                    //    .AddWindows(windows => windows.OnLaunched((app, args) => {
                    //        MauiWinUIApplication.Current.MainWindow.SetIcon("Platforms/Windows/trayicon.ico");
                    //    }));
#endif
                })
                .ConfigureServices((appBuilder, services) =>
                {
                    services.AddBlazorWebView();
#if WINDOWS
                    //services.AddSingleton<ITrayService, WinUI.TrayService>();
                    //services.AddSingleton<INotificationService, WinUI.NotificationService>();
#elif MACCATALYST
                    //services.AddSingleton<ITrayService, MacCatalyst.TrayService>();
                    //services.AddSingleton<INotificationService, MacCatalyst.NotificationService>();
#endif

                    services.AddCore(new HostEnvironment());
                });
        }
    }
}