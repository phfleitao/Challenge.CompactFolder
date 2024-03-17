using CommandLine;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Operations;
using CompactFolder.Cli.Operations.Contracts;
using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CompactFolder.Cli
{
    public class StartupApplication : IStartupApplication
    {
        private readonly IHost _host;

        public StartupApplication(IHost host)
        {
            _host = host;
        }

        public void Dispose()
        {
            _host?.Dispose();
        }

        public async Task<BaseResult> RunAsync(string[] args)
        {
            return await RunWithParser(args);
        }

        private async Task<BaseResult> RunWithParser(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);
            return await result.MapResult(
                async options =>
                {
                    try
                    {
                        var outputTypeOptionsHandlerFactory = _host.Services.GetService<IOutputTypeHandlerFactory>();
                        var handler = outputTypeOptionsHandlerFactory.Create(options.OutputType);
                        var response = await handler.Handle(options);

                        if (!response.IsSuccess)
                        {
                            Console.WriteLine("ERRORS:");
                            foreach (var error in response.Errors)
                            {
                                Console.WriteLine($" - {error.Description}");
                            }
                        }

                        return response;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return Result.Failure(new CompactFolder.Domain.Common.Error("Cli.Argument", ex.Message));
                    }
                    catch (InvalidArgsOptionsException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return Result.Failure(new CompactFolder.Domain.Common.Error("Cli.Argument", ex.Message));
                    }
                    catch (Exception ex)
                    {
                        var logger = _host.Services.GetRequiredService<ILogger<StartupApplication>>();
                        logger.LogError(ex, ex.Message);
                        Console.WriteLine("An unexpected error occurred. See the logs for more details.");
                        return Result.Failure(new CompactFolder.Domain.Common.Error("Cli.General", "An unexpected error occurred. See the logs for more details."));
                    }                    
                },
                errors =>
                {
                    //Already calls help option
                    return Task.FromResult<BaseResult>(
                        Result.Failure(new CompactFolder.Domain.Common.Error("Cli.Argument", "An error occurred while parsing parameters"))
                    );
                }
            );
        }
    }
}
