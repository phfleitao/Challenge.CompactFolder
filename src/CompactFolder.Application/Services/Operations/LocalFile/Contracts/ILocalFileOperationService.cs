using CompactFolder.Application.Services.Operations.LocalFile.Models;
using CompactFolder.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CompactFolder.Application.Services.Operations.LocalFile.Contracts
{
    public interface ILocalFileOperationService
    {
        Task<Result<LocalFileOperationResponse>> ExecuteAsync(LocalFileOperationRequest request,
            CancellationToken cancellationToken = default);
    }
}
