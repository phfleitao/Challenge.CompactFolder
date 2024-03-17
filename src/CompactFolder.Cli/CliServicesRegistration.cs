using CompactFolder.Application.Services.EmailService.Models;
using CompactFolder.Cli.Operations;
using CompactFolder.Cli.Operations.Contracts;
using CompactFolder.Cli.Operations.Email.Contracts;
using CompactFolder.Cli.Operations.Email.Handlers;
using CompactFolder.Cli.Operations.FileShare.Contracts;
using CompactFolder.Cli.Operations.FileShare.Handlers;
using CompactFolder.Cli.Operations.LocalFile.Contracts;
using CompactFolder.Cli.Operations.LocalFile.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CompactFolder.Cli
{
    public static class CliServicesRegistration
    {
        public static IServiceCollection AddCliServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = new EmailSettings();
            configuration.GetSection("AppSettings:EmailSettings").Bind(emailSettings);
            var emailSecretKey = configuration["AppSettings:EmailSettings:PasswordSecretKey"];
            emailSettings.Password = configuration[emailSecretKey];
            services.AddSingleton(Options.Create(emailSettings));

            services.AddSingleton<IOutputTypeHandlerFactory, OutputTypeHandlerFactory>();
            services.AddSingleton<IStartupApplication, StartupApplication>();

            services.AddTransient<ILocalFileOutputTypeHandler, LocalFileOutputTypeHandler>();
            services.AddTransient<IFileShareOutputTypeHandler, FileShareOutputTypeHandler>();
            services.AddTransient<IEmailOutputTypeHandler, EmailOutputTypeHandler>();

            return services;
        }
    }
}
