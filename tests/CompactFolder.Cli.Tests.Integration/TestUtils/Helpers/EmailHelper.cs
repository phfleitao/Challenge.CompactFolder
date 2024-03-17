using CompactFolder.Application.Services.EmailService.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CompactFolder.Cli.Tests.Integration.Helpers
{
    public static class EmailHelper
    {
        public static EmailMessage FindEmlFileByMessageId(string pathDirectory, Guid messageId)
        {
            var emlFiles = GetFilesEml(pathDirectory)
                .OrderByDescending(file => file.CreationTime);

            foreach (var emlFile in emlFiles)
            {
                var message = DeserializeEmlFileToEmailMessage(emlFile);

                if (message.Id == messageId)
                    return message;
            }

            return default(EmailMessage);
        }

        public static IEnumerable<FileInfo> GetFilesEml(string pathDirectory)
        {
            return new DirectoryInfo(pathDirectory)
                .GetFiles("*.eml")
                .AsEnumerable();
        }

        public static EmailMessage DeserializeEmlFileToEmailMessage(FileInfo file)
        {
            var emlContent = File.ReadAllText(file.FullName);
            var emlMessage = MimeMessage.Load(new MemoryStream(Encoding.UTF8.GetBytes(emlContent)));

            if (!Guid.TryParse(emlMessage.Headers[EmailMessage.HEADER_MESSAGE_ID], out Guid messageId))
                messageId = default(Guid);
            
            var outputDirectory = Path.GetDirectoryName(file.FullName);
            var attachments = DeserializeEmlAttachments(emlMessage, outputDirectory);
            SaveAttachmentsToDisk(attachments);

            return new EmailMessage(
                        messageId,
                        emlMessage.To.ToString(),
                        emlMessage.From.ToString(),
                        emlMessage.Subject,
                        emlMessage.TextBody,
                        attachments.FirstOrDefault().FileName); //TODO: Maybe Create Possibility to More than 1 Attachment
        }

        private static IEnumerable<(string FileName, Stream FileStream)> DeserializeEmlAttachments(MimeMessage emlMessage, string baseOutputDirectory)
        {
            var attachmentPathList = new List<(string, Stream)>();

            foreach (var attachment in emlMessage.Attachments)
            {
                if (attachment is MimePart mimePart && Path.HasExtension(mimePart.FileName))
                {
                    var filePath = Path.Combine(baseOutputDirectory, mimePart.FileName);
                    var memoryStream = new MemoryStream();

                    mimePart.Content.DecodeTo(memoryStream);
                    memoryStream.Position = 0;
                    
                    attachmentPathList.Add((filePath, memoryStream));
                }
            }
            return attachmentPathList;
        }

        private static void SaveAttachmentsToDisk(IEnumerable<(string FileName, Stream FileStream)> attachments)
        {
            foreach (var attachment in attachments)
            {
                SaveAttachmentToDisk(attachment.FileName, attachment.FileStream);
            }
        }

        private static void SaveAttachmentToDisk(string fileName, Stream fileStream)
        {
            using (Stream stream = File.Create(fileName))
            {
                fileStream.CopyTo(stream);
            }
        }
    }
}