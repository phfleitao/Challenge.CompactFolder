using CompactFolder.Domain.Base;

namespace CompactFolder.Application.Services.NetworkService.Contracts
{
    public interface INetworkFileSender
    {
        BaseResult Send(string originFilePath, string sharedDestinationFilePath);
    }
}
