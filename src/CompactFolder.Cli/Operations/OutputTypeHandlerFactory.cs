using CompactFolder.Cli.Operations.Contracts;
using CompactFolder.Cli.Operations.Email.Contracts;
using CompactFolder.Cli.Operations.FileShare.Contracts;
using CompactFolder.Cli.Operations.LocalFile.Contracts;
using CompactFolder.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CompactFolder.Cli.Operations
{
    public class OutputTypeHandlerFactory : IOutputTypeHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public OutputTypeHandlerFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IOutputTypeHandler Create(string outputType)
        {
            if (!Enum.TryParse<OutputTypes>(outputType, true, out var selectedOutputType))
            {
                throw new ArgumentException($"Invalid output type: {outputType}");
            }

            switch (selectedOutputType)
            {
                case OutputTypes.LocalFile:
                    return _serviceProvider.GetService<ILocalFileOutputTypeHandler>();
                case OutputTypes.FileShare:
                    return _serviceProvider.GetService<IFileShareOutputTypeHandler>();
                case OutputTypes.Email:
                    return _serviceProvider.GetService<IEmailOutputTypeHandler>();
                default:
                    throw new NotSupportedException($"Unsupported output type: {outputType}");
            }
        }
    }
}
