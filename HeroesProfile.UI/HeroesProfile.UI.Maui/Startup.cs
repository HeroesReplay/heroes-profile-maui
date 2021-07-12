using System.IO;
using System.Net.Http;

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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace HeroesProfile.UI.Maui
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .RegisterBlazorMauiWebView(typeof(Startup).Assembly)
                .UseMicrosoftExtensionsServiceProviderFactory()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                //.ConfigureLifecycleEvents((context, lifecycleBuilder) =>
                //{

                //})
                //.ConfigureAppConfiguration((context, builder) =>
                //{

                //})
                //.ConfigureHostConfiguration((configurationBuilder) =>
                //{

                //})
                .ConfigureServices(services =>
                {
                    Settings settings = new Settings();
                    UserSettings userSettings = new UserSettings();

                    Directory.CreateDirectory(settings.GameTempPath);

                    services.AddSingleton<NavigationManager>();

                    services
                        .AddSingleton(settings)
                        .AddSingleton(userSettings)
                        .AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                        .AddLogging(builder =>
                        {
                            builder.AddDebug();
                        });

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
                        .AddSingleton<GameFileWatcher>()
                        .AddSingleton<StartupScanner>();

                    services.AddMediatR((configuration) => configuration.AsSingleton(), typeof(Session).Assembly);
                });
        }
    }
}