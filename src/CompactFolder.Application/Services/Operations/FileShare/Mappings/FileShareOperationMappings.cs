using CompactFolder.Application.Services.Operations.FileShare.Models;
using CompactFolder.Domain.Operations.FileShare;

namespace CompactFolder.Application.Services.Operations.FileShare.Mappings
{
    public static class FileShareOperationMappings
    {
        public static FileShareOperation FromRequest(this FileShareOperationRequest from)
        {
            return new FileShareOperation(
                from.OriginPath,
                from.OutputFileName,
                from.ExclusionRules,
                from.SharedPath);
        }

        public static FileShareOperationResponse ToResponse(this FileShareOperation from)
        {
            return new FileShareOperationResponse() 
            {
                Id = from.Id,
                OriginPath = from.OriginPath.Path,
                OutputFileName = from.OutputFileName.FileName,
                ExclusionRules = from.ExclusionRules,
                SharedPath = from.SharedPath.Path,
                SharedFullPath = from.SharedFullPath.Path,
                CompressionPath = from.CompressionPath.Path
            };
        }
    }
}
