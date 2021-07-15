using System;
using System.IO;
using System.Text.Json.Serialization;

using HeroesProfile.Core.BackgroundServices;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.CQRS.Behaviours;
using HeroesProfile.Core.Fakes;
using HeroesProfile.Core.JsonConverters;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Parsers;
using HeroesProfile.Core.Repositories;
using HeroesProfile.Core.Watchers;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TwitchLib.Api;
using TwitchLib.Api.Interfaces;

namespace HeroesProfile.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IHostEnvironment environment)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: false)
                .Build();

            AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

            Directory.CreateDirectory(appSettings.GameTempDirectory);
            Directory.CreateDirectory(appSettings.GameDocumentsDirectory);
            Directory.CreateDirectory(appSettings.SimulationTargetDirectory);
            Directory.CreateDirectory(appSettings.SimulationSourceDirectory);
            Directory.CreateDirectory(appSettings.ApplicationDataDirectory);
            Directory.CreateDirectory(appSettings.ApplicationSessionDirectory);

            /*
            * Logging and defaultSettings
            */
            services
                .AddSingleton(configuration)
                .AddSingleton(configuration.GetSection("AppSettings").Get<AppSettings>())
                .AddSingleton(configuration.GetSection("UserSettings").Get<UserSettings>())
                .AddLogging(builder =>
                {
                    if (appSettings.Debug)
                        builder.AddDebug();
                });

            /*
             * Allows us to log each command or query that has executed
             */
            services
                .AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            /*
             * Watch for .battlelobby, .StormSave and .StormReplay files
             * Watch for files that are copied into the Heroes Profile Session Data folder (copied game files)
             */
            services
                .AddSingleton<AbstractGameFileSystemWatcher, BattleLobbySystemWatcher>()
                .AddSingleton<AbstractGameFileSystemWatcher, StormSaveSystemWatcher>()
                .AddSingleton<AbstractGameFileSystemWatcher, StormReplaySystemWatcher>()
                .AddSingleton<SessionFileSystemWatcher>();

            /*
             * Reading and Writing to JSON files or the single tracked Session
             */
            services
                .AddSingleton<SessionRepository>()
                .AddSingleton<ReplaysRepository>()
                .AddSingleton<UserSettingsRepository>();

            services
                .AddSingleton<JsonConverter, ByteArrayToReadableStringConverter>()
                .AddSingleton<JsonConverter, ReplayToReadableStringConverter>()
                .AddSingleton<JsonConverter, FileInfoToFullPathConverter>();

            /*
             * Parsers that parse each type of supported game file the application watches
             */
            services
                .AddSingleton<AggregateReplayParser>()
                .AddSingleton<IReplayParser, StormReplayParser>()
                .AddSingleton<IReplayParser, BattleLobbyParser>()
                .AddSingleton<IReplayParser, StormSaveParser>();

            /*
             * Upload client is used for Uploading Replays to Heroes Profile /Upload endpoint.
             * Talents client is used for the Heroes Profile Twitch Extension using /twitch/extension endpoint.
             */
            services
                .AddSingleton<IUploadClient, UploadClient>()
                .AddSingleton<TalentsClient, TalentsClient>()
                .AddSingleton<ITwitchAPI, TwitchAPI>(provider => new TwitchAPI(provider.GetRequiredService<ILoggerFactory>(), rateLimiter: null, settings: null));

            services.AddSingleton(typeof(ITwitchWrapper), appSettings.EnableFakeTwitch ? typeof(FakeTwitchWrapperClient) : typeof(TwitchWrapperClient));

            var talentsClientBuilder = services
                    .AddHttpClient<TalentsClient>()
                    .ConfigureHttpClient(client => client.BaseAddress = appSettings.HeroesProfileApiUri)
                    .AddPolicyHandler((provider, ctx) => PollyPolicies.GetHeroesProfileRetryPolicy(provider.GetRequiredService<ILogger<TalentsClient>>()))
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            var uploadClientBuilder = services
                .AddHttpClient<IUploadClient, UploadClient>()
                .ConfigureHttpClient(client => client.BaseAddress = appSettings.HeroesProfileApiUri)
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((provider, ctx) => PollyPolicies.GetHeroesProfileRetryPolicy(provider.GetRequiredService<ILogger<UploadClient>>()));

            /*
             * This allows us to fake HTTP responses for HttpClients, for an easier experience in testing and development without needing the real service.
             */
            if (appSettings.EnableFakeHttp)
            {
                services.AddTransient<FakeHeroesProfileDelegatingHandler>();
                talentsClientBuilder.ConfigurePrimaryHttpMessageHandler(provider => provider.GetRequiredService<FakeHeroesProfileDelegatingHandler>());
                uploadClientBuilder.ConfigurePrimaryHttpMessageHandler(provider => provider.GetRequiredService<FakeHeroesProfileDelegatingHandler>());
            }

            /*
             * Hosted services only works with the Hosting Extensions from .NET
             * Maui does not use the same Hosting service and so we much register them differently depending on how we execute background services.
             * If this is for Maui, we must hook into Application startup and run the services.
             * If this is for Console, .AddHostedService will handle starting the services for us.
             */
            if (environment.ApplicationName.Equals("HeroesProfile.Console"))
            {
                services
                    .AddHostedService<GameSimulator>()
                    .AddHostedService<FileWatchers>()
                    .AddHostedService<ReplayProcessor>();
            }
            else
            {
                services
                    .AddSingleton<GameSimulator>()
                    .AddSingleton<FileWatchers>()
                    .AddSingleton<ReplayProcessor>();
            }

            /*
             * Registers all the handlers and behavours.
             */
            services.AddMediatR((mediatorService) => mediatorService.AsSingleton(), typeof(Extensions).Assembly);

            return services;
        }
    }
}
