using CompactFolder.Domain.Common;

namespace CompactFolder.Application.Services.CompressorService
{
    public static class EmailSenderErrors
    {
        public static readonly Error GenericError = new Error("EmailSender.Generic", "Error when trying to send email");
        public static readonly Error SendError = new Error("EmailSender.SendError", "Error when trying to send email");
    }
}
