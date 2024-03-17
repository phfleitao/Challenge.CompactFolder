using CompactFolder.Application.Services.CompressorService.Contracts;
using CompactFolder.Application.Services.FileServices.Contracts;
using CompactFolder.Application.Services.Operations.LocalFile.Contracts;
using CompactFolder.Application.Services.Operations.LocalFile.Mappings;
using CompactFolder.Application.Services.Operations.LocalFile.Models;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using CompactFolder.Domain.Extensions;
using CompactFolder.Domain.Operations.LocalFile;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CompactFolder.Application.Services.Operations.LocalFile
{
    public sealed class LocalFileOperationService : ILocalFileOperationService
    {
        private readonly ILogger<LocalFileOperationService> _logger;
        private readonly ICompressorCreator _compressorCreator;
        private readonly IFileMover _fileMover;

        public LocalFileOperationService(
            ILogger<LocalFileOperationService> logger,
            ICompressorCreator compressorCreator,
            IFileMover fileMover)
        {
            _logger = logger;
            _compressorCreator = compressorCreator;
            _fileMover = fileMover;
        }

        public async Task<Result<LocalFileOperationResponse>> ExecuteAsync(LocalFileOperationRequest request, CancellationToken cancellationToken = default)
        {
            var modelValidator = new LocalFileOperationValidator();
            var model = request.FromRequest();

            var modelValidatorResult = await modelValidator.ValidateAsync(model);
            if (modelValidatorResult.IsFailure)
            {
                return modelValidatorResult.AsFailureResult<LocalFileOperationResponse>();
            }

            var compressResult = CreateZip(model);
            if (compressResult.IsFailure)
            {
                return compressResult.AsFailureResult<LocalFileOperationResponse>();
            }

            var moverResult = MoveCreatedZipToFinalDestination(model);
            if (moverResult.IsFailure)
            {
                return moverResult.AsFailureResult<LocalFileOperationResponse>();
            }

            return Result<LocalFileOperationResponse>.Success(model.ToResponse());
        }
        private BaseResult CreateZip(LocalFileOperation model)
        {
            return _compressorCreator.Create(model.OriginPath.Path, model.CompressionPath.Path, model.ExclusionRules);
        }
        private BaseResult MoveCreatedZipToFinalDestination(LocalFileOperation model)
        {
            return _fileMover.Move(model.CompressionPath.Path, model.DestinationFullPath.Path);
        }        
    }
}
