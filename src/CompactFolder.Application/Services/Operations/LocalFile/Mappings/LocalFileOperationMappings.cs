using CompactFolder.Application.Services.Operations.LocalFile.Models;
using CompactFolder.Domain.Operations.LocalFile;

namespace CompactFolder.Application.Services.Operations.LocalFile.Mappings
{
    public static class LocalFileOperationMappings
    {
        public static LocalFileOperation FromRequest(this LocalFileOperationRequest from)
        {
            return new LocalFileOperation(
                from.OriginPath,
                from.OutputFileName,
                from.ExclusionRules,
                from.DestinationPath);
        }

        public static LocalFileOperationResponse ToResponse(this LocalFileOperation from)
        {
            return new LocalFileOperationResponse() 
            {
                Id = from.Id,
                OriginPath = from.OriginPath.Path,
                OutputFileName = from.OutputFileName.FileName,
                ExclusionRules = from.ExclusionRules,
                DestinationPath = from.DestinationPath.Path,
                DestinationFullPath = from.DestinationFullPath.Path,
                CompressionPath = from.CompressionPath.Path
            };
        }
    }
}
