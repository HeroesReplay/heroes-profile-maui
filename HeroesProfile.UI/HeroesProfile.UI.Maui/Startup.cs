using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using HeroesProfile.Core;
using HeroesProfile.UI.Maui.ViewModels;

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
                .ConfigureLifecycleEvents((context, lifecycle) =>
                {
#if WINDOWS
                    lifecycle
                        .AddWindows(window => window.OnClosed((app, args) =>
                        {
                            Initializer.Stop();
                        }))
                        .AddWindows(window => window.OnLaunched((app, args) =>
                        {
                            Initializer.Start();
                            Platforms.Windows.WindowExtensions.SetIcon(MauiWinUIApplication.Current.MainWindow, "Platforms/Windows/icon.ico");
                        }));
#endif
                })
                .ConfigureServices((appBuilder, services) =>
                {
                    services.AddBlazorWebView();

                    services
                        .AddBlazorise(options =>
                        {
                            options.ChangeTextOnKeyPress = true; // optional
                        })
                        .AddBootstrapProviders()
                        .AddFontAwesomeIcons();
#if WINDOWS
                    services
                        .AddSingleton<ITrayService, Platforms.Windows.TrayService>()
                        .AddSingleton<INotificationService, Platforms.Windows.NotificationService>();
#elif MACCATALYST
                    services
                        .AddSingleton<ITrayService, MacCatalyst.TrayService>()
                        .AddSingleton<INotificationService, MacCatalyst.NotificationService>();
#endif

                    services
                        .AddSingleton<ReplaysViewModel>()
                        .AddSingleton<AnalysisViewModel>()
                        .AddSingleton<SettingsViewModel>();

                    services
                        .AddCoreModule(new HostEnvironment())
                        .AddCoreMediator(typeof(Startup).Assembly);

                });
        }
    }
}