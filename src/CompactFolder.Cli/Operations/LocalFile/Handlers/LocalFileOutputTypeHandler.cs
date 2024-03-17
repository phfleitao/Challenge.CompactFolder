using CompactFolder.Application.Services.Operations.LocalFile.Contracts;
using CompactFolder.Application.Services.Operations.LocalFile.Models;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Extensions;
using CompactFolder.Cli.Operations.LocalFile.Contracts;
using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompactFolder.Cli.Operations.LocalFile.Handlers
{
    public class LocalFileOutputTypeHandler : ILocalFileOutputTypeHandler
    {
        private readonly ILocalFileOperationService _localFileOperationService;

        public LocalFileOutputTypeHandler(ILocalFileOperationService localFileOperationService)
        {
            _localFileOperationService = localFileOperationService;
        }

        public async Task<BaseResult> Handle(Options options)
        {
            ValidateOptions(options);

            var request = CreateRequest(options);
            return await _localFileOperationService.ExecuteAsync(request);
        }

        private void ValidateOptions(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.DestinationPath))
                throw new InvalidArgsOptionsException(options.GetAttributes(nameof(options.DestinationPath)).LongName);
        }

        private LocalFileOperationRequest CreateRequest(Options options)
        {
            return new LocalFileOperationRequest()
            {
                OriginPath = options.OriginPath,
                OutputFileName = options.DestinationFileName,
                ExclusionRules = new List<IExclusionRule>()
                {
                    new FileExtensionExclusionRule(options.ExcludedFileExtensions),
                    new FileNameExclusionRule(options.ExcludedFileNames),
                    new DirectoryNameExclusionRule(options.ExcludedDirectories)
                },
                DestinationPath = options.DestinationPath
            };
        }
    }
}
