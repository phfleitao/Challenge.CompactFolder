using CompactFolder.Application.Services.CompressorService.Contracts;
using CompactFolder.Application.Services.EmailService.Contracts;
using CompactFolder.Application.Services.FileServices.Contracts;
using CompactFolder.Application.Services.NetworkService.Contracts;
using CompactFolder.Infrastructure.CompressorService;
using CompactFolder.Infrastructure.EmailService;
using CompactFolder.Infrastructure.FileService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompactFolder.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IFileMover, FileMover>();
            services.AddSingleton<INetworkFileSender, NetworkFileSender>();

            services.AddTransient<ICompressorCreator, CompressorCreator>();
            services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}
