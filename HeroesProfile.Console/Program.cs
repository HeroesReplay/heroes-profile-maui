using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services;
using HeroesProfile.Core.Services.Background;
using HeroesProfile.Core.Services.Http;
using HeroesProfile.Core.Services.Parsers;
using HeroesProfile.Core.Services.PreProcessors;
using HeroesProfile.Core.Services.Repositories;
using HeroesProfile.Core.Services.Upload;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.RunAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureHostOptions(hostOptions => hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost)
                .UseConsoleLifetime(x => x.SuppressStatusMessages = false)
                .ConfigureServices((hostContext, services) =>
                {
                    Settings settings = new Settings();
                    UserSettings userSettings = new UserSettings();

                    Directory.CreateDirectory(settings.GameTempPath);

                    services
                        .AddSingleton(settings)
                        .AddSingleton(userSettings)
                        .AddLogging()
                        .AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

                    services
                        .AddSingleton(new FileSystemWatcher(settings.GameDocumentsPath, "*.StormSave") { EnableRaisingEvents = true, IncludeSubdirectories = true })
                        .AddSingleton(new FileSystemWatcher(settings.GameDocumentsPath, "*.StormReplay") { EnableRaisingEvents = true, IncludeSubdirectories = true })
                        .AddSingleton(new FileSystemWatcher(settings.GameTempPath, "*.battlelobby") { EnableRaisingEvents = true, IncludeSubdirectories = true });

                    services
                        .AddSingleton<ReplaysRepository>()
                        .AddSingleton<SettingsRepository>();

                    services
                        .AddSingleton<AggregateReplayParser>()
                        .AddSingleton<IReplayParser, StormReplayParser>()
                        .AddSingleton<IReplayParser, BattleLobbyParser>()
                        .AddSingleton<IReplayParser, StormSaveParser>();

                    services
                        .AddSingleton(typeof(HttpMessageHandler), settings.EnableFakeHttp ? typeof(FakeHttpClientHandler) : typeof(HttpClientHandler))
                        .AddSingleton<ReplayUploader.IClient, ReplayUploader.Client>();

                    services.AddSingleton<SessionManager>();
                    services.AddSingleton<StormReplayScanner>();

                    services
                        .AddHostedService<GameFileWatcher>()
                        .AddHostedService<StartupScanner>();

                    services.AddMediatR((configuration) => configuration.AsSingleton(), typeof(Session).Assembly);
                });
        }
    }
}
