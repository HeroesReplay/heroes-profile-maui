using System;
using System.IO;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using MauiApp2.Core;
using MauiApp2.Services;
using MauiApp2.ViewModels;

using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace MauiApp2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureLifecycleEvents((lifecycle) =>
                {
#if WINDOWS
                    lifecycle
                        .AddWindows(configure =>
                        {
                            configure.OnLaunched((app, args) =>
                            {
                                Platforms.Windows.WindowExtensions.SetIcon(MauiWinUIApplication.Current.MainWindow, "Platforms/Windows/icon.ico");

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

                            configure.OnLaunching((app, args) => { });

                            configure.OnActivated((window, args) =>
                            {
                                // System.Windows.MessageBox.Show("Launched :-)");
                            });
                        });
#endif
                });

            builder.Host
                .ConfigureHostConfiguration(builder =>
                {
                    
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddBlazorWebView()
                        .AddBlazorise(options =>
                        {
                            options.ChangeTextOnKeyPress = false;
                            options.DelayTextOnKeyPressInterval = 500;
                            options.DelayTextOnKeyPress = true;
                        })
                        .AddBootstrapProviders()
                        .AddBootstrapComponents()
                        .AddFontAwesomeIcons();

                    /* Temporary Hack */
                    services.Remove(ServiceDescriptor.Scoped(typeof(Blazorise.INotificationService), Type.GetType("Blazorise.NotificationService, Blazorise")));
                    services.Remove(ServiceDescriptor.Scoped(typeof(IMessageService), Type.GetType("Blazorise.MessageService, Blazorise")));
                    services.Remove(ServiceDescriptor.Scoped(typeof(IPageProgressService), Type.GetType("Blazorise.PageProgressService, Blazorise")));

                    /* Temporary Hack */
                    services.Add(ServiceDescriptor.Singleton(typeof(Blazorise.INotificationService), Type.GetType("Blazorise.NotificationService, Blazorise")));
                    services.Add(ServiceDescriptor.Singleton(typeof(IMessageService), Type.GetType("Blazorise.MessageService, Blazorise")));
                    services.Add(ServiceDescriptor.Singleton(typeof(IPageProgressService), Type.GetType("Blazorise.PageProgressService, Blazorise")));

                    services
                       .AddSingleton<MainLayoutViewModel>()
                       .AddSingleton<ReplaysViewModel>()
                       .AddSingleton<AnalysisViewModel>()
                       .AddSingleton<SettingsViewModel>();
                        
#if WINDOWS
                    services.AddSingleton<Services.ITrayService, Platforms.Windows.WindowsTrayService>();
                    services.AddSingleton<Services.INotificationService, Platforms.Windows.WindowsNotificationService>();
#elif MACCATALYST
                services
                    .AddSingleton<ITrayService, MacCatalyst.MacTrayService>()
                    .AddSingleton<IOSNotificationService, MacCatalyst.MacNotificationService>();
#endif

                    services
                        .AddCoreModule(new CustomHostEnvironment())
                        .AddCoreMediator(typeof(MauiProgram).Assembly);

                });

            return builder.Build();
        }
    }
}