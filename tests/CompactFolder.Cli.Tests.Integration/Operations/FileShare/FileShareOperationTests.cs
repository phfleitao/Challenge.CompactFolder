using CommandLine;
using CompactFolder.Application.Services.Operations.FileShare.Models;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Tests.Integration.Helpers;
using CompactFolder.Cli.Tests.Integration.TestUtils.Fixtures.Operations;
using CompactFolder.Domain.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Cli.Tests.Integration.Operations.FileShare
{
    public class FileShareOperationTests : IClassFixture<FileShareOperationTestsFixture>
    {
        
        private readonly FileShareOperationTestsFixture _fixture;

        public Guid TestIdentifier { get; set; } = Guid.NewGuid();
        public string OriginPath { get; set; }
        public string FileName { get; set; }
        public IEnumerable<string> ExcludedExtensions { get; set; }
        public IEnumerable<string> ExcludedFiles { get; set; }
        public IEnumerable<string> ExcludedDirectories { get; set; }
        public string SharedPath { get; set; }
        public string OutputType { get; set; } = "fileShare";

        public FileShareOperationTests(FileShareOperationTestsFixture fixture)
        {
            _fixture = fixture;
            OriginPath = $"{_fixture.BaseZipFolder}_{TestIdentifier}";
            FileName = $"File_{TestIdentifier}.zip";
            SharedPath = _fixture.BaseSharedFolder;

            //Generate random information for each test
            RandomFileSystemGeneratorHelper.GenerateRandomFoldersAndFiles(OriginPath, 3, 5, 5);
            ExcludedExtensions = _fixture.GenerateExcludedExtensions(OriginPath, 3);
            ExcludedFiles = _fixture.GenerateExcludedFileNames(OriginPath, 3);
            ExcludedDirectories = _fixture.GenerateExcludedDirectories(OriginPath, 2);
        }

        [Trait("Integration.Operations", "FileShare")]
        [Fact]
        public async Task GivenValidFileShareOperationWithAllExclusions_ShouldCompactOnlyNotExcludedFilesAndMoveToFinalDestination()
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.ExcludedExtensionsArgument} {string.Join(",", ExcludedExtensions)}" +
                $" {_fixture.ExcludedFilesArgument} {string.Join(",", ExcludedFiles)}" +
                $" {_fixture.ExcludedDirectoriesArgument} {string.Join(",", ExcludedDirectories)}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.SharedPathArgument} {SharedPath}";
            var args = arg.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args) as Result<FileShareOperationResponse>;

            //Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Value.SharedPath.Should().Be(SharedPath);

            //Assert Zip File
            var extractedZipPath = _fixture.ExtractZipFile(response.Value.SharedFullPath);

            _fixture
                .ExcludedExtensionsExistsInPath(extractedZipPath, ExcludedExtensions)
                .Should().BeFalse();
            _fixture
                .ExcludedFileNamesExistsInPath(extractedZipPath, ExcludedFiles)
                .Should().BeFalse();
            _fixture
                .ExcludedDirectoriesExistsInPath(extractedZipPath, ExcludedDirectories)
                .Should().BeFalse();
        }

        [Trait("Integration.Operations", "FileShare")]
        [Fact]
        public async Task GivenValidFileShareOperationWithoutExclusions_ShouldCompactAllFilesAndMoveToFinalDestination()
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.SharedPathArgument} {SharedPath}";
            var args = arg.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args) as Result<FileShareOperationResponse>;

            //Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Value.SharedPath.Should().Be(SharedPath);

            //Assert Zip File
            var extractedZipPath = _fixture.ExtractZipFile(response.Value.SharedFullPath);
            _fixture
                .IsTwoPathsHasIdenticalItems(extractedZipPath, OriginPath)
                .Should().BeTrue();
        }

        [Trait("Integration.Operations", "FileShare")]
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(@"folder/")]
        [InlineData(@"C:\folder\subfolder")]
        public async Task GivenInvalidSharedPath_ShouldReturnFailureResult(string destinationPath)
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.SharedPathArgument} {destinationPath}";
            var args = arg.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args);

            //Assert
            response.IsFailure.Should().BeTrue();
            response.Errors.Should().NotBeNull();
            response.Errors.Should().HaveCountGreaterThan(0);
        }
    }
}
