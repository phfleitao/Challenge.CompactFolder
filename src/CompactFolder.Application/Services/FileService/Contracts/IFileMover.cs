using CompactFolder.Domain.Base;

namespace CompactFolder.Application.Services.FileServices.Contracts
{
    public interface IFileMover
    {
        BaseResult Move(string originPath, string destinationPath);
    }
}
