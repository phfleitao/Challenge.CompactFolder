using CompactFolder.Application.Services.Operations.Email;
using CompactFolder.Application.Services.Operations.Email.Contracts;
using CompactFolder.Application.Services.Operations.FileShare;
using CompactFolder.Application.Services.Operations.FileShare.Contracts;
using CompactFolder.Application.Services.Operations.LocalFile;
using CompactFolder.Application.Services.Operations.LocalFile.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace CompactFolder.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ILocalFileOperationService, LocalFileOperationService>();
            services.AddTransient<IFileShareOperationService, FileShareOperationService>();
            services.AddTransient<IEmailOperationService, EmailOperationService>();

            return services;
        }
    }
}