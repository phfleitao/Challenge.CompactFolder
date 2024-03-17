using CompactFolder.Application.Services.EmailService.Models;
using CompactFolder.Application.Services.Operations.Email.Models;
using CompactFolder.Domain.Operations.Email;

namespace CompactFolder.Application.Services.Operations.Email.Mappings
{
    public static class EmailOperationMappings
    {
        public static EmailOperation FromRequest(this EmailOperationRequest from)
        {
            return new EmailOperation(
                from.OriginPath,
                from.OutputFileName,
                from.ExclusionRules,
                from.EmailTo);
        }

        public static EmailOperationResponse ToResponse(this EmailOperation from)
        {
            return new EmailOperationResponse() 
            {
                Id = from.Id,
                OriginPath = from.OriginPath.Path,
                OutputFileName = from.OutputFileName.FileName,
                ExclusionRules = from.ExclusionRules,
                To = from.To.Address,
                Subject = from.Subject,
                Body = from.Body,
                AttachmentFilePath = from.AttachmentFilePath.Path                
            };
        }

        public static EmailMessage ToEmailMessage(this EmailOperation from)
        {
            return new EmailMessage()
            {
                Id = from.Id,
                To = from.To.Address,
                Subject = from.Subject,
                Body = from.Body,
                Attachment = from.AttachmentFilePath.Path
            };
        }
    }
}
