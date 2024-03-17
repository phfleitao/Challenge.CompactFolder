using System;

namespace CompactFolder.Application.Services.EmailService.Models
{
    public class EmailMessage
    {
        public const string HEADER_MESSAGE_ID = "MessageId";

        public Guid Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachment { get; set; }

        public EmailMessage() { }
        public EmailMessage(Guid id, string to, string from, string subject, string body, string attachment = "")
        {
            Id = id;
            To = to;
            From = from;
            Subject = subject;
            Body = body;
            Attachment = attachment;
        }
    }
}
