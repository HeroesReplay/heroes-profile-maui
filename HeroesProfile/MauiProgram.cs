using System.Text.Json.Serialization;
using Blazorise;
using Blazorise.Icons.FontAwesome;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Maui.LifecycleEvents;
using Blazorise.Bootstrap5;
using HeroesProfile.UI.Services.Notification;
using HeroesProfile.UI.Services.Tray;
using HeroesProfile.UI.Core;
using HeroesProfile.UI.Core.BackgroundServices;
using HeroesProfile.UI.Core.Clients;
using HeroesProfile.UI.Core.CQRS.Behaviours;
using HeroesProfile.UI.Core.Fakes;
using HeroesProfile.UI.Core.JsonConverters;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Parsers;
using HeroesProfile.UI.Core.Repositories;
using HeroesProfile.UI.Core.Watchers;
using HeroesProfile.UI.Services;
using HeroesProfile.UI.Services.LifeCycle;
using HeroesProfile.UI.ViewModels;

namespace HeroesProfile.UI;

public class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder(useDefaults: true)
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); })
            .ConfigureLifecycleEvents(Lifecycle.AddPlatformEvents);
      
        builder.Services.AddMauiBlazorWebView();
        
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services
            .AddBlazorise(options => options.Immediate = true)
            .AddBootstrap5Providers()
            .AddBootstrap5Components()
            .AddFontAwesomeIcons();

        builder.Services
            .AddSingleton<MainLayoutViewModel>()
            .AddSingleton<ReplaysViewModel>()
            .AddSingleton<AnalysisViewModel>()
            .AddSingleton<SettingsViewModel>();

        builder.Services.AddSingleton<IPlatformTrayService, PlatformTrayService>();
        builder.Services.AddSingleton<IPlatformNotificationService, PlatformNotificationService>();

        builder.Services.AddSingleton<IFileSystem>(FileSystem.Current);
        builder.Services.AddSingleton<IPreferences>(Preferences.Default);

        builder.Services.AddMediatR(config =>
        {
#if DEBUG
            config.BehaviorsToRegister.Add(ServiceDescriptor.Singleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>)));
#endif            
            config.RegisterServicesFromAssemblyContaining<MauiProgram>();
        });

        AddCore(builder);

        return builder.Build();
    }

    public static MauiAppBuilder AddCore(MauiAppBuilder builder)
    {
        AppSettings appSettings = new AppSettings()
        {
            EnableFakeHttp = false,
            ClearStoredReplaysOnStart = true
        };

        Directory.CreateDirectory(appSettings.GameTempDirectory);
        Directory.CreateDirectory(appSettings.GameDocumentsDirectory);
        Directory.CreateDirectory(appSettings.ApplicationDataDirectory);
        Directory.CreateDirectory(appSettings.ApplicationSessionDirectory);

        builder.Services
            .AddSingleton(builder.Configuration)
            .AddSingleton(appSettings)
            .AddSingleton(new UserSettings()
            {
                EnablePostMatch = false,
                EnablePreMatch = false
            });

        builder.Services
            .AddSingleton<AbstractGameFileSystemWatcher, BattleLobbySystemWatcher>()
            .AddSingleton<AbstractGameFileSystemWatcher, StormSaveSystemWatcher>()
            .AddSingleton<AbstractGameFileSystemWatcher, StormReplaySystemWatcher>()
            .AddSingleton<SessionFileSystemWatcher>();

        builder.Services
            .AddSingleton<SessionRepository>()
            .AddSingleton<ReplaysRepository>()
            .AddSingleton<UserSettingsRepository>();

        builder.Services
            .AddSingleton<JsonConverter, ByteArrayToReadableStringConverter>()
            .AddSingleton<JsonConverter, ReplayToReadableStringConverter>()
            .AddSingleton<JsonConverter, FileInfoToFullPathConverter>();

        builder.Services
            .AddTransient<AggregateReplayParser>()
            .AddTransient<IReplayParser, StormReplayParser>()
            .AddTransient<IReplayParser, BattleLobbyParser>()
            .AddTransient<IReplayParser, StormSaveParser>();

        builder.Services
            .AddSingleton<IUploadClient, UploadClient>()
            .AddSingleton<PreMatchClient>();

        var preMatchClientBuilder = builder.Services
            .AddHttpClient<PreMatchClient>()
            .ConfigureHttpClient(client => client.BaseAddress = appSettings.HeroesProfileUri)
            .AddPolicyHandler((provider, ctx) => PollyPolicies.GetHeroesProfileRetryPolicy(provider.GetRequiredService<ILogger<PreMatchClient>>()))
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        var uploadClientBuilder = builder.Services
            .AddHttpClient<IUploadClient, UploadClient>()
            .ConfigureHttpClient(client => client.BaseAddress = appSettings.HeroesProfileApiUri)
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((provider, ctx) => PollyPolicies.GetHeroesProfileRetryPolicy(provider.GetRequiredService<ILogger<UploadClient>>()));

        if (appSettings.EnableFakeHttp)
        {
            builder.Services.AddTransient<FakeHeroesProfileDelegatingHandler>();
            preMatchClientBuilder.ConfigurePrimaryHttpMessageHandler(provider => provider.GetRequiredService<FakeHeroesProfileDelegatingHandler>());
            uploadClientBuilder.ConfigurePrimaryHttpMessageHandler(provider => provider.GetRequiredService<FakeHeroesProfileDelegatingHandler>());
        }

        builder.Services
            .AddSingleton<FileWatchers>()
            .AddSingleton<OnLaunchReplayProcessor>();

        return builder;
    }
}