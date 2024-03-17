using CompactFolder.Application.Services.EmailService.Models;
using CompactFolder.Cli.Tests.Integration.Helpers;
using CompactFolder.Cli.Tests.Integration.TestUtils.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace CompactFolder.Cli.Tests.Integration.TestUtils.Fixtures.Operations
{
    public class EmailOperationTestsFixture : OperationTestsFixture
    {
        private string _pickupDirectoryLocation = string.Empty;
        public string BaseEmailTestsTempFolder { get; set; }

        public string EmailToArgument { get; set; } = "--emailTo";

        public EmailOperationTestsFixture()
        {
            ProcessConfigurationFromEmailSettings();
            ConfigureEmailTestsTempFolder();
        }

        public EmailMessage GetEmailMessageFromFile(Guid messageId)
        {
            return EmailHelper.FindEmlFileByMessageId(_pickupDirectoryLocation, messageId);
        }

        public string ExtractEmailMessageAttachment(string compactedAttachmentFile)
        {
            var zipFile = compactedAttachmentFile;
            var baseFileFolder = Path.GetDirectoryName(zipFile);
            var extractFolderName = Path.GetFileNameWithoutExtension(zipFile);
            var outputPath = Path.Combine(baseFileFolder, extractFolderName);
            FileHelper.UnzipFolder(zipFile, outputPath);
            return outputPath;
        }

        private void ProcessConfigurationFromEmailSettings()
        {
            var emailSettings = Host.Services.GetService<IOptions<EmailSettings>>().Value;
            var pickupDirectoryLocation = emailSettings.PickupDirectoryLocation;

            var expandedPickupDirectoryLocation = Environment.ExpandEnvironmentVariables(pickupDirectoryLocation);

            _pickupDirectoryLocation = expandedPickupDirectoryLocation;
        }
        private void ConfigureEmailTestsTempFolder()
        {
            BaseEmailTestsTempFolder = Path.Combine(BaseTestsTempFolder, "EmailTests");
            BaseZipFolder = Path.Combine(BaseEmailTestsTempFolder, @"BaseFolder");

            if (!Directory.Exists(BaseEmailTestsTempFolder))
                Directory.CreateDirectory(BaseEmailTestsTempFolder);
        }
        private void ClearEmailTestsTempFolder()
        {
            FileHelper.ClearFolderSkippingProcessingFiles(BaseEmailTestsTempFolder);
        }

        #region IDisposable Members

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearEmailTestsTempFolder();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}