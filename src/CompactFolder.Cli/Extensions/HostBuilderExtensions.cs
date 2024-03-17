using CompactFolder.Application;
using CompactFolder.Domain;
using CompactFolder.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace CompactFolder.Cli.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddConfiguration(this IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((hostContext, config) =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                    .Build();

                config.AddConfiguration(configuration);
            });
            return builder;
        }

        public static IHostBuilder AddServices(this IHostBuilder builder)
        {
            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddCliServices(hostContext.Configuration)
                        .AddInfrastructureServices(hostContext.Configuration)
                        .AddApplicationServices()
                        .AddDomainServices();
            });
            return builder;
        }

        public static IHostBuilder AddLogging(this IHostBuilder builder)
        {
            builder.ConfigureServices((hostContext, services) =>
            {
                var appSettings = hostContext.Configuration.GetSection("AppSettings");
                services.AddLogging(logBuilder =>
                {
                    logBuilder.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                    logBuilder.AddConsole();
                    logBuilder.AddDebug();

                    // Adjust the minimum log level based on AppSettings
                    var logLevel = appSettings["LogLevel"];
                    if (Enum.TryParse<LogLevel>(logLevel, out var parsedLogLevel))
                    {
                        logBuilder.SetMinimumLevel(parsedLogLevel);
                    }
                    else
                    {
                        logBuilder.SetMinimumLevel(LogLevel.Information);
                    }
                });
            });
            return builder;
        }
    }
}
