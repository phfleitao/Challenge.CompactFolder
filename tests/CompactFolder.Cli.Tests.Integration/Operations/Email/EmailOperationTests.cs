using CommandLine;
using CompactFolder.Application.Services.Operations.Email.Models;
using CompactFolder.Cli.Tests.Integration.Helpers;
using CompactFolder.Cli.Tests.Integration.TestUtils.Fixtures.Operations;
using CompactFolder.Domain.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Cli.Tests.Integration.Operations.Email
{
    public class EmailOperationTests : IClassFixture<EmailOperationTestsFixture>
    {
        private readonly EmailOperationTestsFixture _fixture;

        public Guid TestIdentifier { get; set; } = Guid.NewGuid();
        public string OriginPath { get; set; }
        public string FileName { get; set; }
        public IEnumerable<string> ExcludedExtensions { get; set; }
        public IEnumerable<string> ExcludedFiles { get; set; }
        public IEnumerable<string> ExcludedDirectories { get; set; }
        public string EmailTo { get; set; } = "test123@email.com";
        public string OutputType { get; set; } = "email";

        public EmailOperationTests(EmailOperationTestsFixture fixture)
        {
            _fixture = fixture;
            OriginPath = $"{_fixture.BaseZipFolder}_{TestIdentifier}";
            FileName = $"File_{TestIdentifier}.zip";

            //Generate random information for each test
            RandomFileSystemGeneratorHelper.GenerateRandomFoldersAndFiles(OriginPath, 3, 5, 5);
            ExcludedExtensions = _fixture.GenerateExcludedExtensions(OriginPath, 3);
            ExcludedFiles = _fixture.GenerateExcludedFileNames(OriginPath, 3);
            ExcludedDirectories = _fixture.GenerateExcludedDirectories(OriginPath, 2);
        }

        [Trait("Integration.Operations", "Email")]
        [Fact]
        public async Task GivenValidEmailOperationWithAllExclusions_ShouldCompactOnlyNotExcludedFilesAndSendEmail()
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.ExcludedExtensionsArgument} {string.Join(",", ExcludedExtensions)}" +
                $" {_fixture.ExcludedFilesArgument} {string.Join(",", ExcludedFiles)}" +
                $" {_fixture.ExcludedDirectoriesArgument} {string.Join(",", ExcludedDirectories)}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.EmailToArgument} {EmailTo}";
            var args = arg.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args) as Result<EmailOperationResponse>;


            var message = _fixture.GetEmailMessageFromFile(response.Value.Id);
            var extractedAttachmentPath = _fixture.ExtractZipFile(message.Attachment);

            //Assert
            response.IsSuccess.Should().BeTrue();

            //Assert Mail Message
            message.To.Should().Be(response.Value.To);
            message.Subject.Should().Be(response.Value.Subject);
            message.Body.Should().Be(response.Value.Body);
            message.Attachment.Should().EndWith(".zip");

            //Assert Zip Attachment
            _fixture
                .ExcludedExtensionsExistsInPath(extractedAttachmentPath, ExcludedExtensions)
                .Should().BeFalse();
            _fixture
                .ExcludedFileNamesExistsInPath(extractedAttachmentPath, ExcludedFiles)
                .Should().BeFalse();
            _fixture
                .ExcludedDirectoriesExistsInPath(extractedAttachmentPath, ExcludedDirectories)
                .Should().BeFalse();
        }

        [Trait("Integration.Operations", "Email")]
        [Fact]
        public async Task GivenValidEmailOperationWithoutExclusions_ShouldCompactAllFilesAndSendEmail()
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.EmailToArgument} {EmailTo}";
            var args = arg.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args) as Result<EmailOperationResponse>;

            var message = _fixture.GetEmailMessageFromFile(response.Value.Id);
            var extractedAttachmentPath = _fixture.ExtractZipFile(message.Attachment);

            //Assert
            response.IsSuccess.Should().BeTrue();

            //Assert Mail Message
            message.To.Should().Be(response.Value.To);
            message.Subject.Should().Be(response.Value.Subject);
            message.Body.Should().Be(response.Value.Body);
            message.Attachment.Should().EndWith(".zip");

            //Assert Zip Attachment
            _fixture
                .IsTwoPathsHasIdenticalItems(extractedAttachmentPath, OriginPath)
                .Should().BeTrue();
        }

        [Trait("Integration.Operations", "Email")]
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc@@.com")]
        public async Task GivenInvalidEmailOperation_ShouldReturnParserFailureResult(string emailTo)
        {
            //Arrange
            var arg = $"{_fixture.InputPathArgument} {OriginPath}" +
                $" {_fixture.OutputFileNameArgument} {FileName}" +
                $" {_fixture.OutputTypeArgument} {OutputType}" +
                $" {_fixture.EmailToArgument} {emailTo}";
            var args = arg.SplitArgs();

            //Act
            var response = await _fixture.StartupApplication.RunAsync(args);

            //Assert
            response.IsFailure.Should().BeTrue();
            response.FirstError.Code.Should().Be("Cli.Argument");
        }
    }
}
