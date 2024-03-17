using CompactFolder.Application.Services.Operations.Email.Contracts;
using CompactFolder.Application.Services.Operations.Email.Models;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Extensions;
using CompactFolder.Cli.Operations.Email.Contracts;
using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompactFolder.Cli.Operations.Email.Handlers
{
    public class EmailOutputTypeHandler : IEmailOutputTypeHandler
    {
        private readonly IEmailOperationService _emailOperationService;

        public EmailOutputTypeHandler(IEmailOperationService emailOperationService)
        {
            _emailOperationService = emailOperationService;
        }

        public async Task<BaseResult> Handle(Options options)
        {
            ValidateOptions(options);

            var request = CreateRequest(options);
            return await _emailOperationService.ExecuteAsync(request);
        }

        private void ValidateOptions(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.EmailTo))
                throw new InvalidArgsOptionsException(options.GetAttributes(nameof(options.EmailTo)).LongName);
        }

        private EmailOperationRequest CreateRequest(Options options)
        {
            return new EmailOperationRequest()
            {
                OriginPath = options.OriginPath,
                OutputFileName = options.DestinationFileName,
                ExclusionRules = new List<IExclusionRule>()
                {
                    new FileExtensionExclusionRule(options.ExcludedFileExtensions),
                    new FileNameExclusionRule(options.ExcludedFileNames),
                    new DirectoryNameExclusionRule(options.ExcludedDirectories)
                },
                EmailTo = options.EmailTo
            };
        }
    }
}
