using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using Microsoft.Extensions.DependencyInjection;
using System;

using Microsoft.AspNetCore.Components.WebView.Maui;

using HeroesProfile.Maui.ViewModels;
using HeroesProfile.Core;
using HeroesProfile.Maui.Platforms.Windows;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace HeroesProfile.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .RegisterBlazorMauiWebView()
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

                            configure.OnLaunching((app, args) => { });

                            configure.OnActivated((window, args) =>
                            {
                                // System.Windows.MessageBox.Show("Launched :-)");
                            });
                        });
#endif
                });

            builder.Host.ConfigureServices((context, services) =>
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

                services.Remove(ServiceDescriptor.Scoped(typeof(INotificationService), Type.GetType("Blazorise.NotificationService, Blazorise")));
                services.Remove(ServiceDescriptor.Scoped(typeof(IMessageService), Type.GetType("Blazorise.MessageService, Blazorise")));
                services.Remove(ServiceDescriptor.Scoped(typeof(IPageProgressService), Type.GetType("Blazorise.PageProgressService, Blazorise")));

                services.Add(ServiceDescriptor.Singleton(typeof(INotificationService), Type.GetType("Blazorise.NotificationService, Blazorise")));
                services.Add(ServiceDescriptor.Singleton(typeof(IMessageService), Type.GetType("Blazorise.MessageService, Blazorise")));
                services.Add(ServiceDescriptor.Singleton(typeof(IPageProgressService), Type.GetType("Blazorise.PageProgressService, Blazorise")));

#if WINDOWS
                services
                        .AddSingleton<ITrayService, WindowsTrayService>()
                        .AddSingleton<IOSNotificationService, WindowsNotificationService>();
#elif MACCATALYST
                services
                    .AddSingleton<ITrayService, MacCatalyst.MacTrayService>()
                    .AddSingleton<IOSNotificationService, MacCatalyst.MacNotificationService>();
#endif

                services
                    .AddSingleton<MainLayoutViewModel>()
                    .AddSingleton<ReplaysViewModel>()
                    .AddSingleton<AnalysisViewModel>()
                    .AddSingleton<SettingsViewModel>();

                services
                    .AddCoreModule(new CustomHostEnvironment())
                    .AddCoreMediator(typeof(MauiProgram).Assembly);

            });

            return builder.Build();
        }
    }
}