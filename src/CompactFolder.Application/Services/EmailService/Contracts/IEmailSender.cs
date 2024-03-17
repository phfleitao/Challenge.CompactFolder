using CompactFolder.Application.Services.EmailService.Models;
using CompactFolder.Domain.Base;
using System.Threading.Tasks;

namespace CompactFolder.Application.Services.EmailService.Contracts
{
    public interface IEmailSender
    {
        Task<BaseResult> SendAsync(EmailMessage emailMessage);
    }
}
