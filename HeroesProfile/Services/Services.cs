using System.Text.Json.Serialization;
using HeroesProfile.Core;
using HeroesProfile.Core.BackgroundServices;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.Fakes;
using HeroesProfile.Core.JsonConverters;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Parsers;
using HeroesProfile.Core.Repositories;
using HeroesProfile.Core.Watchers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;

namespace HeroesProfile.UI.Services;

public static class Services
{
    public static MauiAppBuilder AddCoreModule(this MauiAppBuilder builder)
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