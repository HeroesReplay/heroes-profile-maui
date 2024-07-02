using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using HeroesProfile.Blazor.ViewModels;
using HeroesProfile.Core;
using HeroesProfile.Core.CQRS.Behaviours;
using HeroesProfile.UI.Services;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

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
            .ConfigureLifecycleEvents((lifecycle) =>
            {
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
                                // window.SetIcon("Platforms/Windows/Images/logo.ico");
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


        builder.Configuration.AddJsonFile("appsettings.json", optional: false);
        builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddBlazorise(options =>
            {
                // options.ChangeTextOnKeyPress = false;                        
                // options.DelayTextOnKeyPressInterval = 500;
                // options.DelayTextOnKeyPress = true;
                options.Immediate = true;
            })
            .AddBootstrapProviders()
            .AddBootstrapComponents()
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
#endif

        builder.Services.AddMediatR(config =>
        {
            config.BehaviorsToRegister.Add(ServiceDescriptor.Singleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>)));
            config.RegisterServicesFromAssemblyContaining<Module>();
            config.RegisterServicesFromAssemblyContaining<MauiProgram>();
        });

        builder.AddCoreModule();

        return builder.Build();
    }
}