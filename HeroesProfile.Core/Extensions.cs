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
using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.HttpCallHandlers;
using TwitchLib.Api.Interfaces;

namespace HeroesProfile.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, bool hostedServices = true)
        {
            Settings settings = new Settings();
            UserSettings userSettings = new UserSettings();

            Directory.CreateDirectory(settings.GameTempDirectory);
            Directory.CreateDirectory(settings.GameDocumentsDirectory);
            Directory.CreateDirectory(settings.StoredReplaysDirectory);

            if (settings.Debug)
            {
                // Clean out all previously processed and stored replays.
                // This will start a full scan / import
                File.WriteAllText(Path.Combine(settings.StoredReplaysDirectory, "replays.json"), "[]");
            }


            services
                .AddSingleton(settings)
                .AddSingleton(userSettings)
                .AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddLogging(builder =>
                {
                    if (settings.Debug)
                        builder.AddDebug();
                });

            services
                .AddSingleton(new FileSystemWatcher(settings.GameDocumentsDirectory, "*.StormSave") { EnableRaisingEvents = true, IncludeSubdirectories = true })
                .AddSingleton(new FileSystemWatcher(settings.GameDocumentsDirectory, "*.StormReplay") { EnableRaisingEvents = true, IncludeSubdirectories = true })
                .AddSingleton(new FileSystemWatcher(settings.GameTempDirectory, "*.battlelobby") { EnableRaisingEvents = true, IncludeSubdirectories = true });

            services
                .AddSingleton<SessionRepository>()
                .AddSingleton<ReplaysRepository>()
                .AddSingleton<UserSettingsRepository>();

            services
                .AddSingleton<JsonConverter, ByteArrayToReadableStringConverter>()
                .AddSingleton<JsonConverter, ReplayToReadableStringConverter>()
                .AddSingleton<JsonConverter, FileInfoToFullPathConverter>();

            services
                .AddSingleton<AggregateReplayParser>()
                .AddSingleton<IReplayParser, StormReplayParser>()
                .AddSingleton<IReplayParser, BattleLobbyParser>()
                .AddSingleton<IReplayParser, StormSaveParser>();

            services
                .AddSingleton<IUploadClient, UploadClient>()
                .AddSingleton<TalentsClient, TalentsClient>()
                .AddSingleton<ITwitchAPI, TwitchAPI>(provider => new TwitchAPI(provider.GetRequiredService<ILoggerFactory>(), rateLimiter: null, settings: null));


            if (settings.EnableFakeHttp)
            {
                services.AddTransient<FakeHeroesProfileDelegatingHandler>();

                services
                    .AddHttpClient<TalentsClient>()
                    .ConfigureHttpClient(client => client.BaseAddress = settings.HeroesProfileApiUri)
                    .AddHttpMessageHandler(provider => provider.GetRequiredService<FakeHeroesProfileDelegatingHandler>())
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                    .AddPolicyHandler(PollyPolicies.GetHeroesProfileRetryPolicy());

                services
                    .AddHttpClient<IUploadClient, UploadClient>()
                    .ConfigureHttpClient(client => client.BaseAddress = settings.HeroesProfileApiUri)
                    .AddHttpMessageHandler(provider => provider.GetRequiredService<FakeHeroesProfileDelegatingHandler>())
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                    .AddPolicyHandler(PollyPolicies.GetHeroesProfileRetryPolicy());
            }
            else
            {
                services
                    .AddHttpClient<TalentsClient>()
                    .ConfigureHttpClient(client => client.BaseAddress = settings.HeroesProfileApiUri)
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                    .AddPolicyHandler(PollyPolicies.GetHeroesProfileRetryPolicy());

                services
                    .AddHttpClient<IUploadClient, UploadClient>()
                    .ConfigureHttpClient(client => client.BaseAddress = settings.HeroesProfileApiUri)
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                    .AddPolicyHandler(PollyPolicies.GetHeroesProfileRetryPolicy());
            }


            if (hostedServices)
            {
                services
                .AddHostedService<FileWatcherService>()
                .AddHostedService<ReplayProcessingService>();
            }
            else
            {
                services
                .AddSingleton<FileWatcherService>()
                .AddSingleton<ReplayProcessingService>();
            }

            services.AddMediatR((mediatorService) => mediatorService.AsSingleton(), typeof(Extensions).Assembly);

            return services;
        }
    }
}
