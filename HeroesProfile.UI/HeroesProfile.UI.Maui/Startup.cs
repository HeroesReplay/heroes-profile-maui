using System;

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
                .ConfigureImageSources(images =>
                {

                })
                .ConfigureFonts(fonts =>
                {

                })
                .EnableHotReload()
                .ConfigureHostConfiguration(configBuilder =>
                {

                })
                .ConfigureLifecycleEvents((context, lifecycle) =>
                {
#if WINDOWS
                    lifecycle
                        .AddWindows(configure =>
                        {
                            configure.OnLaunched((app, args) =>
                            {
                                Platforms.Windows.WindowExtensions.SetIcon(MauiWinUIApplication.Current.MainWindow, "Platforms/Windows/icon.ico");

                                MauiWinUIApplication.Current.MainWindow.SizeChanged += (sender, e) =>
                                {
                                    App.Current.MainPage.WidthRequest = e.Size.Width;
                                    App.Current.MainPage.HeightRequest = e.Size.Height;
                                };

                                Initializer.Start();
                            });

                            configure.OnVisibilityChanged((window, args) =>
                            {
                                if (!args.Visible)
                                {
                                    Platforms.Windows.WindowExtensions.MinimizeToTray(window);
                                }
                            });

                            configure.OnClosed((window, args) =>
                            {
                                Initializer.Stop();

                                //if (window == MauiWinUIApplication.Current.MainWindow)
                                //{
                                //    args.Handled = true;
                                //}
                                //else
                                //{
                                    
                                //}

                            });

                            configure.OnLaunching((app, args) =>
                            {

                            });

                            configure.OnActivated((window, args) =>
                            {
                                // System.Windows.MessageBox.Show("Launched :-)");
                            });

                        });
#endif
                })
                .ConfigureServices((appBuilder, services) =>
                {
                    services.AddBlazorWebView();

                    services
                        .AddBlazorise(options =>
                        {
                            options.ChangeTextOnKeyPress = false;
                            options.DelayTextOnKeyPressInterval = 500;
                            options.DelayTextOnKeyPress = true;
                        })
                        .AddBootstrapComponents()
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