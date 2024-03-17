using CompactFolder.Application.Services.Operations.Email.Models;
using CompactFolder.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CompactFolder.Application.Services.Operations.Email.Contracts
{
    public interface IEmailOperationService
    {
        Task<Result<EmailOperationResponse>> ExecuteAsync(EmailOperationRequest request,
            CancellationToken cancellationToken = default);
    }
}
