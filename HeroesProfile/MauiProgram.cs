using System.Reflection;
using HeroesProfile.Blazor.ViewModels;
using HeroesProfile.Core;
using HeroesProfile.Core.CQRS.Behaviours;
using HeroesProfile.UI.Services;

using Blazorise;
using Blazorise.Icons.FontAwesome;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Maui.LifecycleEvents;
using Blazorise.Bootstrap5;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;

namespace HeroesProfile.UI;

public class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder(useDefaults: true)
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); })
            .ConfigureEssentials((eb) =>
            {
                
            })
            .ConfigureDispatching()
            .ConfigureLifecycleEvents((lifecycle) =>
            {
                #if MACCATALYST
                lifecycle.AddiOS(configure =>
                {
                    
                });
            
                #endif
                
                #if WINDOWS
                lifecycle
                    .AddWindows(configure =>
                    {
                        configure.OnWindowCreated(x =>
                        {

                        });

                        configure.OnLaunched((app, args) =>
                        {
                            foreach (IWindow window in MauiWinUIApplication.Current.Application.Windows)
                            {
                                if (window is Window w)
                                {
                                    // Platforms.Windows.WindowExtensions.SetIcon(w, "Platforms/Windows/Images/logo.ico");
                                }
                            }

                            Initializer.Start();
                        });

                        configure.OnVisibilityChanged((window, args) =>
                        {
                            if (!args.Visible)
                            {
                                // window.MinimizeToTray();
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
            });

        builder.Configuration.AddJsonStream(FileSystem.OpenAppPackageFileAsync("appsettings.json").Result);
        builder.Configuration.AddJsonStream(FileSystem.OpenAppPackageFileAsync("appsettings.Development.json").Result);
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services
            .AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrap5Providers()
            .AddBootstrap5Components()
            .AddFontAwesomeIcons();

        builder.Services
            .AddSingleton<MainLayoutViewModel>()
            .AddSingleton<ReplaysViewModel>()
            .AddSingleton<AnalysisViewModel>()
            .AddSingleton<SettingsViewModel>();

#if WINDOWS
        builder.Services.AddSingleton<HeroesProfile.UI.Services.ITrayService, HeroesProfile.UI.Platforms.Windows.WindowsTrayService>();
        builder.Services.AddSingleton<HeroesProfile.UI.Services.INotificationService, HeroesProfile.UI.Platforms.Windows.WindowsNotificationService>();
#elif MACCATALYST
        builder.Services.AddSingleton<ITrayService, HeroesProfile.UI.Platforms.Services.TrayService>();
        builder.Services.AddSingleton<HeroesProfile.UI.Services.INotificationService, HeroesProfile.UI.Platforms.Services.NotificationService>();
#endif

        builder.Services.AddMediatR(config =>
        {
#if DEBUG
            config.BehaviorsToRegister.Add(ServiceDescriptor.Singleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>)));
#endif            
            config.RegisterServicesFromAssemblyContaining<IMarker>();
            config.RegisterServicesFromAssemblyContaining<MauiProgram>();
        });

        builder.AddCoreModule();

        return builder.Build();
    }
}