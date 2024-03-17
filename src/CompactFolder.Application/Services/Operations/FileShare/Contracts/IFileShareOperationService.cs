using CompactFolder.Application.Services.Operations.FileShare.Models;
using CompactFolder.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CompactFolder.Application.Services.Operations.FileShare.Contracts
{
    public interface IFileShareOperationService
    {
        Task<Result<FileShareOperationResponse>> ExecuteAsync(FileShareOperationRequest request,
            CancellationToken cancellationToken = default);
    }
}
