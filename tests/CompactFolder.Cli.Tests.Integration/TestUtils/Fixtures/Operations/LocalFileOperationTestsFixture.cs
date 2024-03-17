using CompactFolder.Cli.Tests.Integration.TestUtils.Helpers;
using System.IO;

namespace CompactFolder.Cli.Tests.Integration.TestUtils.Fixtures.Operations
{
    public class LocalFileOperationTestsFixture : OperationTestsFixture
    {
        public string BaseLocalFileTestsTempFolder { get; set; }

        public string DestinationPathArgument { get; set; } = "--destPath";

        public LocalFileOperationTestsFixture()
        {
            ConfigureLocalFileTestsTempFolder();
        }

        private void ConfigureLocalFileTestsTempFolder()
        {
            BaseLocalFileTestsTempFolder = Path.Combine(BaseTestsTempFolder, "LocalFileTests");
            BaseZipFolder = Path.Combine(BaseLocalFileTestsTempFolder, @"BaseFolder");

            if (!Directory.Exists(BaseLocalFileTestsTempFolder))
                Directory.CreateDirectory(BaseLocalFileTestsTempFolder);
        }
        private void ClearLocalFileTestsTempFolder()
        {
            FileHelper.ClearFolderSkippingProcessingFiles(BaseLocalFileTestsTempFolder);
        }

        #region IDisposable Members

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearLocalFileTestsTempFolder();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}