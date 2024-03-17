using CompactFolder.Application.Services.CompressorService.Contracts;
using CompactFolder.Application.Services.EmailService.Contracts;
using CompactFolder.Application.Services.Operations.Email.Contracts;
using CompactFolder.Application.Services.Operations.Email.Mappings;
using CompactFolder.Application.Services.Operations.Email.Models;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using CompactFolder.Domain.Extensions;
using CompactFolder.Domain.Operations.Email;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CompactFolder.Application.Services.Operations.Email
{
    public sealed class EmailOperationService : IEmailOperationService
    {
        private readonly ILogger<EmailOperationService> _logger;
        private readonly ICompressorCreator _compressorCreator;
        private readonly IEmailSender _emailSender;

        public EmailOperationService(
            ILogger<EmailOperationService> logger,
            ICompressorCreator compressorCreator,
            IEmailSender emailSender)
        {
            _logger = logger;
            _compressorCreator = compressorCreator;
            _emailSender = emailSender;
        }

        public async Task<Result<EmailOperationResponse>> ExecuteAsync(EmailOperationRequest request, CancellationToken cancellationToken = default)
        {
            var modelValidator = new EmailOperationValidator();
            var model = request.FromRequest();

            var modelValidatorResult = await modelValidator.ValidateAsync(model);
            if (modelValidatorResult.IsFailure)
            {
                return modelValidatorResult.AsFailureResult<EmailOperationResponse>();
            }

            var compressResult = CreateZip(model);
            if (compressResult.IsFailure)
            {
                return compressResult.AsFailureResult<EmailOperationResponse>();
            }

            var emailResult = await SendEmail(model);
            if (emailResult.IsFailure)
            {
                return emailResult.AsFailureResult<EmailOperationResponse>();
            }

            return Result<EmailOperationResponse>.Success(model.ToResponse());
        }

        private BaseResult CreateZip(EmailOperation model)
        {
            return _compressorCreator.Create(model.OriginPath.Path, model.CompressionPath.Path, model.ExclusionRules);
        }

        private async Task<BaseResult> SendEmail(EmailOperation operation)
        {
            return await _emailSender.SendAsync(operation.ToEmailMessage());
        }
    }
}
