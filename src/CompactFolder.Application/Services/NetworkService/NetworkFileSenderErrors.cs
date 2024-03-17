using CompactFolder.Domain.Common;

namespace CompactFolder.Application.Services.NetworkService
{
    public static class NetworkFileSenderErrors
    {
        public static readonly Error GenericError = new Error("NetworkFileSender.Generic", "Error when trying to copy file to the network path");
    }
}
