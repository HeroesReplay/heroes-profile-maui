using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;

using Microsoft.Extensions.Hosting;

using Polly;

namespace HeroesProfile.Core.BackgroundServices
{
    public class GameSimulator : BackgroundService
    {
        private readonly AppSettings appSettings;

        public GameSimulator(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!appSettings.EnableFileSimulator) return;

            var target = new DirectoryInfo(appSettings.SimulationTargetDirectory);
            target.Delete(recursive: true);
            target.Create();

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var source in new DirectoryInfo(appSettings.SimulationSourceDirectory).GetFiles().OrderBy(x => DateTime.ParseExact((string)x.Name.Split('.')[0], appSettings.DateTimeFormat, CultureInfo.InvariantCulture)))
                {
                    var destination = source.Extension.Equals(".battlelobby")
                        ? Path.Combine(appSettings.GameTempDirectory, "replay.server.battlelobby")
                        : Path.Combine(appSettings.SimulationTargetDirectory, source.Name);

                    //Policy.Handle<IOException>()
                    //    .WaitAndRetry(5, (retryAttempt) => TimeSpan.FromSeconds(0.5))
                    //    .Execute((x) => File.Delete(destination), stoppingToken);

                    byte[] data = await File.ReadAllBytesAsync(source.FullName, stoppingToken);
                    await File.WriteAllBytesAsync(destination, data, stoppingToken);

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            target.Delete(recursive: true);
        }
    }
}