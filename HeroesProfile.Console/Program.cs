using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using HeroesProfile.Core;
using HeroesProfile.Core.CQRS.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HeroesProfile.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                IMediator mediator = host.Services.GetRequiredService<IMediator>();

                await mediator.Send(new InitializeApp.Command());

                await host.RunAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureHostOptions(hostOptions => hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost)
                .ConfigureHostConfiguration((builder) =>
                {

                })
                .UseConsoleLifetime(x => x.SuppressStatusMessages = false)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddCore(hostContext.HostingEnvironment);
                });
        }
    }
}
