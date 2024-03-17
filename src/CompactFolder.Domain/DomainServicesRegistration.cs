using CompactFolder.Domain.Operations;
using CompactFolder.Domain.Operations.Email;
using CompactFolder.Domain.Operations.FileShare;
using CompactFolder.Domain.Operations.LocalFile;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CompactFolder.Domain
{
    public static class DomainServicesRegistration
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {          
            services.AddScoped<AbstractValidator<EmailOperation>, EmailOperationValidator>();
            services.AddScoped<AbstractValidator<LocalFileOperation>, LocalFileOperationValidator>();
            services.AddScoped<AbstractValidator<FileShareOperation>, FileShareOperationValidator>();

            return services;
        }
    }
}
