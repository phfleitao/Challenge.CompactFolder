using CommandLine;
using CompactFolder.Application.Services.Operations.LocalFile.Models;
using CompactFolder.Cli.Tests.Integration.Helpers;
using CompactFolder.Cli.Tests.Integration.TestUtils.Fixtures.Operations;
using CompactFolder.Domain.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Cli.Tests.Integration.Operations.LocalFile
{
    public class LocalFileOperationTests : IClassFixture<LocalFileOperationTestsFixture>
    {
        
        private readonly LocalFileOperationTestsFixture _fixture;

        public Guid TestIdentifier { get; set; } = Guid.NewGuid();
        public string OriginPath { get; set; }
        public string FileName { get; set; }
        public IEnumerable<string> ExcludedExtensions { get; set; }
        public IEnumerable<string> ExcludedFiles { get; set; }
        public IEnumerable<string> ExcludedDirectories { get; set; }
        public string DestinationPath { get; set; }
        public string OutputType { get; set; } = "localFile";

        public LocalFileOperationTests(LocalFileOperationTestsFixture fixture)
        {
            _fixture = fixture;
            OriginPath = $"{_fixture.BaseZipFolder}_{TestIdentifier}";
            FileName = $"File_{TestIdentifier}.zip";
            DestinationPath = _fixture.BaseLocalFileTestsTempFolder;

            //Generate random information for each test
            RandomFileSystemGeneratorHelper.GenerateRandomFoldersAndFiles(OriginPath, 3, 5, 5);
            ExcludedExtensions = _fixture.GenerateExcludedExtensions(OriginPath, 3);
            ExcludedFiles = _fixture.GenerateExcludedFileNames(OriginPath, 3);
            ExcludedDirectories = _fixture.GenerateExcludedDirectories(OriginPath, 2);
        }

        [Trait("Integration.Operations", "LocalFile")]
        [Fact]
        public async Task GivenValidLocalFileOperationWithAllExclusions_ShouldCompactOnlyNotExcludedFilesAndMoveToFinalDestination()
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.ExcludedExtensionsArgument} {string.Join(",", ExcludedExtensions)}" +
                $" {_fixture.ExcludedFilesArgument} {string.Join(",", ExcludedFiles)}" +
                $" {_fixture.ExcludedDirectoriesArgument} {string.Join(",", ExcludedDirectories)}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.DestinationPathArgument} {DestinationPath}";
            var args = arg.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args) as Result<LocalFileOperationResponse>;

            //Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Value.DestinationPath.Should().Be(DestinationPath);

            //Assert Zip File
            var extractedZipPath = _fixture.ExtractZipFile(response.Value.DestinationFullPath);

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

        [Trait("Integration.Operations", "LocalFile")]
        [Fact]
        public async Task GivenValidLocalFileOperationWithoutExclusions_ShouldCompactAllFilesAndMoveToFinalDestination()
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.DestinationPathArgument} {DestinationPath}";
            var args = arg.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args) as Result<LocalFileOperationResponse>;

            //Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Value.DestinationPath.Should().Be(DestinationPath);

            //Assert Zip File
            var extractedZipPath = _fixture.ExtractZipFile(response.Value.DestinationFullPath);
            _fixture
                .IsTwoPathsHasIdenticalItems(extractedZipPath, OriginPath)
                .Should().BeTrue();
        }

        [Trait("Integration.Operations", "LocalFile")]
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(@"folder/")]
        [InlineData(@"\\folder\subfolder")]
        public async Task GivenInvalidDestinationPath_ShouldReturnFailureResult(string destinationPath)
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.DestinationPathArgument} {destinationPath}";
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
