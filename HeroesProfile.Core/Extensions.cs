using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;

using HeroesProfile.Core.BackgroundServices;
using HeroesProfile.Core.CQRS.Behaviours;
using HeroesProfile.Core.Http;
using HeroesProfile.Core.JsonConverters;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Parsers;
using HeroesProfile.Core.Repositories;
using HeroesProfile.Core.Upload;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, bool hostedServices = true)
        {
            Settings settings = new Settings();
            UserSettings userSettings = new UserSettings();

            Directory.CreateDirectory(settings.GameTempPath);
            Directory.CreateDirectory(settings.GameDocumentsPath);

            File.WriteAllText(settings.StoredReplaysPath, string.Empty);

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
                .AddSingleton(new FileSystemWatcher(settings.GameDocumentsPath, "*.StormSave") { EnableRaisingEvents = true, IncludeSubdirectories = true })
                .AddSingleton(new FileSystemWatcher(settings.GameDocumentsPath, "*.StormReplay") { EnableRaisingEvents = true, IncludeSubdirectories = true })
                .AddSingleton(new FileSystemWatcher(settings.GameTempPath, "*.battlelobby") { EnableRaisingEvents = true, IncludeSubdirectories = true });

            services
                .AddSingleton<SessionRepository>()
                .AddSingleton<ReplaysRepository>()
                .AddSingleton<SettingsRepository>();

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
                .AddSingleton(typeof(HttpMessageHandler), settings.EnableFakeHttp ? typeof(FakeHttpClientHandler) : typeof(HttpClientHandler))
                .AddSingleton<UploadReplay.IClient, UploadReplay.Client>();

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
