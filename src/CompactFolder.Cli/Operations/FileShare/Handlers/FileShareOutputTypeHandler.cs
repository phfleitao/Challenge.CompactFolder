using CompactFolder.Application.Services.Operations.FileShare.Contracts;
using CompactFolder.Application.Services.Operations.FileShare.Models;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Extensions;
using CompactFolder.Cli.Operations.FileShare.Contracts;
using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompactFolder.Cli.Operations.FileShare.Handlers
{
    public class FileShareOutputTypeHandler : IFileShareOutputTypeHandler
    {
        private readonly IFileShareOperationService _fileShareOperationService;

        public FileShareOutputTypeHandler(IFileShareOperationService fileShareOperationService)
        {
            _fileShareOperationService = fileShareOperationService;
        }

        public async Task<BaseResult> Handle(Options options)
        {
            ValidateOptions(options);

            var request = CreateRequest(options);
            return await _fileShareOperationService.ExecuteAsync(request);
        }

        private void ValidateOptions(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.SharedPath))
                throw new InvalidArgsOptionsException(options.GetAttributes(nameof(options.SharedPath)).LongName);
        }

        private FileShareOperationRequest CreateRequest(Options options)
        {
            return new FileShareOperationRequest()
            {
                OriginPath = options.OriginPath,
                OutputFileName = options.DestinationFileName,
                ExclusionRules = new List<IExclusionRule>()
                {
                    new FileExtensionExclusionRule(options.ExcludedFileExtensions),
                    new FileNameExclusionRule(options.ExcludedFileNames),
                    new DirectoryNameExclusionRule(options.ExcludedDirectories)
                },
                SharedPath = options.SharedPath
            };
        }
    }
}
