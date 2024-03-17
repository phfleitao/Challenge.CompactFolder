using CompactFolder.Cli.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace CompactFolder.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder();
            var app = builder
                        .AddConfiguration()
                        .AddLogging()
                        .AddServices()
                        .Build();

            var startupApplication = app.Services.GetService<IStartupApplication>();
            
            await startupApplication.RunAsync(args);
        }
    }
}
