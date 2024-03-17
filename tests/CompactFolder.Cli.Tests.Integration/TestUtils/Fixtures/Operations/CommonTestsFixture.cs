using CompactFolder.Cli.Extensions;
using CompactFolder.Cli.Tests.Integration.TestUtils.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompactFolder.Cli.Tests.Integration.TestUtils.Fixtures.Operations
{
    public class OperationTestsFixture : IDisposable
    {
        public IHost Host { get; protected set; }
        public IStartupApplication StartupApplication { get; protected set; }

        public string BaseTestsTempFolder { get; protected set; }
        public string BaseZipFolder { get; protected set; }

        public string InputPathArgument { get; set; } = "-i";
        public string OutputFileNameArgument { get; set; } = "-o";
        public string ExcludedExtensionsArgument { get; set; } = "--ex";
        public string ExcludedFilesArgument { get; set; } = "--ef";
        public string ExcludedDirectoriesArgument { get; set; } = "--ed";
        public string OutputTypeArgument { get; set; } = "-t";

        public OperationTestsFixture()
        {
            Host = CreateHost();
            StartupApplication = Host.Services.GetRequiredService<IStartupApplication>();
            ConfigureTestsTempFolder();
        }
        ~OperationTestsFixture() => Dispose(false);

        private void ConfigureTestsTempFolder()
        {
            BaseTestsTempFolder = Path.Combine(Path.GetTempPath(), @"CompactFolderTests\Integration_Cli");

            if (!Directory.Exists(BaseTestsTempFolder))
                Directory.CreateDirectory(BaseTestsTempFolder);

            BaseZipFolder = Path.Combine(BaseTestsTempFolder, @"BaseFolder");
        }

        protected void ClearTestsTempFolder()
        {
            FileHelper.ClearFolderSkippingProcessingFiles(BaseTestsTempFolder);
        }

        public IEnumerable<string> GenerateExcludedExtensions(string baseFolder, int quantity)
        {
            var distinctExtensions = FileHelper.GetDistinctFileExtensions(baseFolder);
            return FileHelper.SelectRandomItems(distinctExtensions, quantity).ToList();
        }

        public IEnumerable<string> GenerateExcludedFileNames(string baseFolder, int quantity)
        {
            var distinctFileNames = FileHelper.GetDistinctFileNamesWithoutExtension(baseFolder);
            return FileHelper.SelectRandomItems(distinctFileNames, quantity).ToList();
        }

        public IEnumerable<string> GenerateExcludedDirectories(string baseFolder, int quantity)
        {
            var distinctDirectoryNames = FileHelper.GetDistinctDirectoryNames(baseFolder);
            return FileHelper.SelectRandomItems(distinctDirectoryNames, quantity).ToList();
        }

        public bool ExcludedExtensionsExistsInPath(string baseFolder, IEnumerable<string> excludedExtensions)
        {
            return FileHelper.DoesAnyFileExtensionsExistInPath(baseFolder, excludedExtensions);
        }

        public bool IsTwoPathsHasIdenticalItems(string leftFolder, string rightFolder)
        {
            return FileHelper.IsFolderEquals(leftFolder, rightFolder);
        }

        public bool ExcludedFileNamesExistsInPath(string baseFolder, IEnumerable<string> excludedFilesNames)
        {
            return FileHelper.DoesAnyFileNameExistInPath(baseFolder, excludedFilesNames);
        }

        public bool ExcludedDirectoriesExistsInPath(string baseFolder, IEnumerable<string> excludedDirectories)
        {
            return FileHelper.DoesAnyDirectoryExistInPath(baseFolder, excludedDirectories);
        }

        public string ExtractZipFile(string compactedZipFilePath)
        {
            var baseFileFolder = Path.GetDirectoryName(compactedZipFilePath);
            var extractFolderName = Path.GetFileNameWithoutExtension(compactedZipFilePath);
            var outputPath = Path.Combine(baseFileFolder, extractFolderName);
            FileHelper.UnzipFolder(compactedZipFilePath, outputPath);
            return outputPath;
        }

        private IHost CreateHost()
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
            builder
                .UseEnvironment("Testing")
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.test.json", optional: true)
                        .Build();

                    config.AddConfiguration(configuration);
                })
                .AddServices();
            return builder.Build();
        }

        #region IDisposable

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Host?.Dispose();
                    StartupApplication?.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}