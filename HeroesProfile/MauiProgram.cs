using System.Text.Json.Serialization;
using HeroesProfile.Blazor.ViewModels;
using HeroesProfile.Core;
using HeroesProfile.Core.CQRS.Behaviours;

using Blazorise;
using Blazorise.Icons.FontAwesome;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Maui.LifecycleEvents;
using Blazorise.Bootstrap5;
using HeroesProfile.UI.Services.Notification;
using HeroesProfile.UI.Services.Tray;
using HeroesProfile.Core.BackgroundServices;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.Fakes;
using HeroesProfile.Core.JsonConverters;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Parsers;
using HeroesProfile.Core.Repositories;
using HeroesProfile.Core.Watchers;
using HeroesProfile.UI.Services;
using HeroesProfile.UI.Services.LifeCycle;

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
            .ConfigureLifecycleEvents(Lifecycle.AddPlatformEvents);

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

        builder.Services.AddSingleton<IPlatformTrayService, PlatformTrayService>();
        builder.Services.AddSingleton<IPlatformNotificationService, PlatformNotificationService>();

        builder.Services.AddMediatR(config =>
        {
#if DEBUG
            config.BehaviorsToRegister.Add(ServiceDescriptor.Singleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>)));
#endif            
            config.RegisterServicesFromAssemblyContaining<IMarker>();
            config.RegisterServicesFromAssemblyContaining<MauiProgram>();
        });

        AddCore(builder);

        return builder.Build();
    }

    public static MauiAppBuilder AddCore(MauiAppBuilder builder)
    {
        AppSettings appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()!;
        UserSettings defaultUserSettings = builder.Configuration.GetSection("UserSettings").Get<UserSettings>()!;

        Directory.CreateDirectory(appSettings.GameTempDirectory);
        Directory.CreateDirectory(appSettings.GameDocumentsDirectory);
        Directory.CreateDirectory(appSettings.SimulationTargetDirectory);
        Directory.CreateDirectory(appSettings.SimulationSourceDirectory);
        Directory.CreateDirectory(appSettings.ApplicationDataDirectory);
        Directory.CreateDirectory(appSettings.ApplicationSessionDirectory);

        builder.Services
            .AddSingleton(builder.Configuration)
            .AddSingleton(appSettings)
            .AddSingleton(defaultUserSettings);

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