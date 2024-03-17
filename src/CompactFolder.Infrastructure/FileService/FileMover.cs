using CompactFolder.Application.Services.CompressorService;
using CompactFolder.Application.Services.FileServices.Contracts;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace CompactFolder.Infrastructure.FileService
{
    public class FileMover : IFileMover
    {
        private readonly ILogger<FileMover> _logger;
        public FileMover(ILogger<FileMover> logger)
        {
            _logger = logger;
        }
        public BaseResult Move(string originPath, string destinationPath)
        {
            try
            {
                File.Move(originPath, destinationPath);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failure(CompressorCreatorErrors.GenericError);
            }
        }
    }
}
