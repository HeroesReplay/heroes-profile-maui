using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using HeroesProfile.Core;
using HeroesProfile.UI.Platforms.Windows;
using HeroesProfile.UI.Services;
using HeroesProfile.Blazor.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace HeroesProfile.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder                
            .UseMauiApp<App>()
            .ConfigureEssentials(essentialsBuilder => 
            { 
                    
            })
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
                        configure.OnWindowCreated(x => 
                        {
                            
                        });                                                        

                        configure.OnLaunched((app, args) =>
                        {                                   
                            foreach(IWindow window in MauiWinUIApplication.Current.Application.Windows)
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

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddMauiBlazorWebView();
                
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

        /* Temporary Hack */
        //builder.Services.Remove(ServiceDescriptor.Scoped(typeof(Blazorise.INotificationService), Type.GetType("Blazorise.NotificationService, Blazorise")));
        //builder.Services.Remove(ServiceDescriptor.Scoped(typeof(IMessageService), Type.GetType("Blazorise.MessageService, Blazorise")));
        //builder.Services.Remove(ServiceDescriptor.Scoped(typeof(IPageProgressService), Type.GetType("Blazorise.PageProgressService, Blazorise")));

        /* Temporary Hack */
        //builder.Services.Add(ServiceDescriptor.Singleton(typeof(Blazorise.INotificationService), Type.GetType("Blazorise.NotificationService, Blazorise")));
        //builder.Services.Add(ServiceDescriptor.Singleton(typeof(IMessageService), Type.GetType("Blazorise.MessageService, Blazorise")));
        //builder.Services.Add(ServiceDescriptor.Singleton(typeof(IPageProgressService), Type.GetType("Blazorise.PageProgressService, Blazorise")));

        builder.Services
           .AddSingleton<MainLayoutViewModel>()
           .AddSingleton<ReplaysViewModel>()
           .AddSingleton<AnalysisViewModel>()
           .AddSingleton<SettingsViewModel>();

#if WINDOWS
        builder.Services.AddSingleton<Services.ITrayService, WindowsTrayService>();
        builder.Services.AddSingleton<Services.INotificationService, WindowsNotificationService>();
#elif MACCATALYST
            services
                .AddSingleton<ITrayService, MacCatalyst.MacTrayService>()
                .AddSingleton<IOSNotificationService, MacCatalyst.MacNotificationService>();
#endif

        builder.Services
            .AddCoreModule(new CustomHostEnvironment())
            .AddCoreMediator(typeof(MauiProgram).Assembly);


        return builder.Build();
    }
}