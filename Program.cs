using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Minsky
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureLogging(loggerFactory => loggerFactory.AddEventLog())
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<BotWorker>();
            });
    }
}
