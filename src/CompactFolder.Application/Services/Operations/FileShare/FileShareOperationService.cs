using CompactFolder.Application.Services.CompressorService.Contracts;
using CompactFolder.Application.Services.NetworkService.Contracts;
using CompactFolder.Application.Services.Operations.FileShare.Contracts;
using CompactFolder.Application.Services.Operations.FileShare.Mappings;
using CompactFolder.Application.Services.Operations.FileShare.Models;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using CompactFolder.Domain.Extensions;
using CompactFolder.Domain.Operations.FileShare;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CompactFolder.Application.Services.Operations.FileShare
{
    public sealed class FileShareOperationService : IFileShareOperationService
    {
        private readonly ILogger<FileShareOperationService> _logger;
        private readonly ICompressorCreator _compressorCreator;
        private readonly INetworkFileSender _networkFileSender;

        public FileShareOperationService(
            ILogger<FileShareOperationService> logger,
            ICompressorCreator compressorCreator,
            INetworkFileSender networkFileSender)
        {
            _logger = logger;
            _compressorCreator = compressorCreator;
            _networkFileSender = networkFileSender;
        }

        public async Task<Result<FileShareOperationResponse>> ExecuteAsync(FileShareOperationRequest request, CancellationToken cancellationToken = default)
        {
            var modelValidator = new FileShareOperationValidator();
            var model = request.FromRequest();

            var modelValidatorResult = await modelValidator.ValidateAsync(model);
            if (modelValidatorResult.IsFailure)
            {
                return modelValidatorResult.AsFailureResult<FileShareOperationResponse>();
            }

            var compressResult = CreateZip(model);
            if (compressResult.IsFailure)
            {
                return compressResult.AsFailureResult<FileShareOperationResponse>();
            }

            var networkCopyResult = CopyCreatedZipToNetworkSharedPath(model);
            if (networkCopyResult.IsFailure)
            {
                return networkCopyResult.AsFailureResult<FileShareOperationResponse>();
            }

            return Result<FileShareOperationResponse>.Success(model.ToResponse());
        }

        private BaseResult CreateZip(FileShareOperation model)
        {
            return _compressorCreator.Create(model.OriginPath.Path, model.CompressionPath.Path, model.ExclusionRules);
        }
        private BaseResult CopyCreatedZipToNetworkSharedPath(FileShareOperation model)
        {
            return _networkFileSender.Send(model.CompressionPath.Path, model.SharedFullPath.Path);
        }
    }
}
