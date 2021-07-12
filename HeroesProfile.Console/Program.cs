using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using HeroesProfile.Core;

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
                    services.AddCore(hostedServices: true);
                });
        }
    }
}
