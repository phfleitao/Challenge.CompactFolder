using CompactFolder.Domain.Common;

namespace CompactFolder.Application.Services.FileService
{
    public static class FileMoverErrors
    {
        public static readonly Error GenericError = new Error("FileMover.Generic", "Error when trying to move file between paths");
    }
}
