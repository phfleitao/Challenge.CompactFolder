using CompactFolder.Domain.Common;

namespace CompactFolder.Application.Services.CompressorService
{
    public static class CompressorCreatorErrors
    {
        public static readonly Error GenericError = new Error("CompressorCreator.Generic", "Error when trying to compress the path");
        public static readonly Error OriginNotFound = new Error("CompressorCreator.OriginNotFound", "Origin path not found!");
        public static readonly Error DestinationNotFound = new Error("CompressorCreator.DestinationNotFound", "Destination path not found!");
    }
}
