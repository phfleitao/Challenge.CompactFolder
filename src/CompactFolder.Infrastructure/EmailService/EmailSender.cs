using CompactFolder.Application.Services.CompressorService;
using CompactFolder.Application.Services.EmailService.Contracts;
using CompactFolder.Application.Services.EmailService.Models;
using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CompactFolder.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<BaseResult> SendAsync(EmailMessage emailMessage)
        {
            try
            {
                Email.DefaultSender = GetConfiguredSender();

                var email = Email
                    .From(_emailSettings.FromEmail)
                    .To(emailMessage.To)
                    .Subject(emailMessage.Subject)
                    .Body(emailMessage.Body)
                    .Header(EmailMessage.HEADER_MESSAGE_ID, emailMessage.Id.ToString());

                //TODO: Review settings for content-type and filename
                if (!string.IsNullOrEmpty(emailMessage.Attachment))
                    email.AttachFromFilename(emailMessage.Attachment, "application/zip", Path.GetFileName(emailMessage.Attachment));

                var sendEmailResponse = await email.SendAsync();

                if (!sendEmailResponse.Successful)
                    return Result.Failure(EmailSenderErrors.SendError);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failure(EmailSenderErrors.GenericError);
            }
        }

        private SmtpSender GetConfiguredSender()
        {
            var smtpClient = new SmtpClient(_emailSettings.Server, _emailSettings.Port);
            smtpClient.EnableSsl = _emailSettings.EnableSsl;
            smtpClient.DeliveryMethod = ConvertToSmtpDeliveryMethod(_emailSettings.DeliveryMethod);

            if (!string.IsNullOrWhiteSpace(_emailSettings.PickupDirectoryLocation))
            {
                //To expand directory if is set like e.g. %TEMP%\MyDir => C:\Users\[USER]\AppData\Local\Temp\MyDir
                var expandedPickupDirectoryLocation = Environment.ExpandEnvironmentVariables(_emailSettings.PickupDirectoryLocation);
                smtpClient.PickupDirectoryLocation = expandedPickupDirectoryLocation;
            }

            if (!_emailSettings.UseDefaultCredentials)
            {
                smtpClient.UseDefaultCredentials = _emailSettings.UseDefaultCredentials;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
            }

            return new SmtpSender(smtpClient);
        }
        
        private SmtpDeliveryMethod ConvertToSmtpDeliveryMethod(string deliveryMethodValue)
        {
            SmtpDeliveryMethod smtpDeliveryMethod;

            if (Enum.TryParse<SmtpDeliveryMethod>(_emailSettings.DeliveryMethod, true, out smtpDeliveryMethod))
                return smtpDeliveryMethod;

            return SmtpDeliveryMethod.Network;
        }
    }
}
