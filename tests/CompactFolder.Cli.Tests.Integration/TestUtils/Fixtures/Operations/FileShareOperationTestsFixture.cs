using CompactFolder.Cli.Tests.Integration.TestUtils.Helpers;
using System;
using System.IO;

namespace CompactFolder.Cli.Tests.Integration.TestUtils.Fixtures.Operations
{
    public class FileShareOperationTestsFixture : OperationTestsFixture
    {
        private const string SHARE_NAME = "FileShareTests";
        public string BaseFileShareTestsTempFolder { get; set; }
        public string BaseSharedFolderPhysicalPath { get; set; }
        public string BaseSharedFolder { get; set; }
        public string SharedPathArgument { get; set; } = "--sharedPath";

        public FileShareOperationTestsFixture()
        {
            ConfigureFileShareTestsTempFolder();
        }

        private void ConfigureFileShareTestsTempFolder()
        {
            BaseFileShareTestsTempFolder = Path.Combine(BaseTestsTempFolder, "FileShareTests");
            BaseSharedFolderPhysicalPath = Path.Combine(BaseFileShareTestsTempFolder, "SharedFolder");
            BaseSharedFolder = $@"\\{Environment.MachineName}\{SHARE_NAME}";
            BaseZipFolder = Path.Combine(BaseFileShareTestsTempFolder, @"BaseFolder");

            if (!Directory.Exists(BaseFileShareTestsTempFolder))
                Directory.CreateDirectory(BaseFileShareTestsTempFolder);

            NetworkHelper.ShareFolder(BaseSharedFolderPhysicalPath, SHARE_NAME);
        }
        private void ClearFileShareTestsTempFolder()
        {
            NetworkHelper.UnshareFolder(SHARE_NAME);
            FileHelper.ClearFolderSkippingProcessingFiles(BaseFileShareTestsTempFolder);
        }

        #region IDisposable Members

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearFileShareTestsTempFolder();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}