using CompactFolder.Application.Services.CompressorService;
using CompactFolder.Application.Services.CompressorService.Contracts;
using CompactFolder.Application.Services.EmailService.Contracts;
using CompactFolder.Application.Services.EmailService.Models;
using CompactFolder.Application.Services.Operations.Email;
using CompactFolder.Application.Services.Operations.Email.Contracts;
using CompactFolder.Application.Services.Operations.Email.Models;
using CompactFolder.Domain.Common;
using CompactFolder.Domain.Operations;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Application.Tests.Unit.Services.Operations.Email
{
    public class EmailOperationServiceTests
    {
        private ILogger<EmailOperationService> _logger;

        private IEmailSender _emailSenderService;
        private ICompressorCreator _compressorCreatorService;
        private IEmailOperationService _emailOperationService;
        public EmailOperationRequest Request { get; set; }
        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public TEmail EmailTo { get; set; }

        public EmailOperationServiceTests()
        {
            _logger = Substitute.For<ILogger<EmailOperationService>>();

            _compressorCreatorService = Substitute.For<ICompressorCreator>();
            _emailSenderService = Substitute.For<IEmailSender>();

            _emailOperationService = new EmailOperationService(_logger, _compressorCreatorService, _emailSenderService);

            OriginPath = @"C:\example\file.txt";
            OutputFileName = @"file.zip";
            ExclusionRules = new List<IExclusionRule> {
                new FileExtensionExclusionRule(new string[] { ".txt", ".jpg" }),
                new FileNameExclusionRule(new string[] { "excludedFile" }),
                new DirectoryNameExclusionRule(new string[] { "excludedDirectory" })
            };
            EmailTo = @"pedro@example.com";

            Request = new EmailOperationRequest()
            {
                OriginPath = OriginPath.Path,
                OutputFileName = OutputFileName.Path,
                ExclusionRules = ExclusionRules,
                EmailTo = EmailTo.Address
            };
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_ValidEmailOperation_ShouldCompactAndSendEmail()
        {
            // Arrange          
            var expectedAttachmentPath = Path.Combine(Path.GetTempPath(), Request.OutputFileName);
            _compressorCreatorService.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<IExclusionRule>>()).Returns(Result.Success());
            _emailSenderService.SendAsync(Arg.Any<EmailMessage>()).Returns(Result.Success());

            // Act
            var response = await _emailOperationService.ExecuteAsync(Request);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Errors.Should().HaveCount(0);
            response.Value.AttachmentFilePath.Should().Contain(expectedAttachmentPath);
            _compressorCreatorService.Received().Create(
                Arg.Is(Request.OriginPath), 
                Arg.Is(expectedAttachmentPath), 
                Arg.Is(Request.ExclusionRules));
            await _emailSenderService.Received().SendAsync(
                Arg.Is<EmailMessage>(m => m.To.Equals(Request.EmailTo)));
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_InvalidEmailToNull_ShouldThrowException()
        {
            // Arrange & Act       
            var request = new EmailOperationRequest() 
            { 
                OriginPath = OriginPath.Path,
                OutputFileName = OutputFileName.Path,
                ExclusionRules = ExclusionRules,
                EmailTo = null
            };

            // Act
            Func<Task> action = async () => await _emailOperationService.ExecuteAsync(request);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_InvalidModelValidation_ShouldReturnResultError()
        {
            // Arrange & Act
            var request = new EmailOperationRequest()
            {
                OriginPath = OriginPath.Path,
                OutputFileName = "file.txt",
                ExclusionRules = ExclusionRules,
                EmailTo = EmailTo.Address
            };

            // Act
            var actual = await _emailOperationService.ExecuteAsync(request);

            // Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.NotZipFileExtension.Code).Should().BeTrue();
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_ErrorWhenCompressFile_ShouldReturnResultError()
        {
            // Arrange & Act
            _compressorCreatorService.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<IExclusionRule>>())
                .Returns(Result.Failure(CompressorCreatorErrors.GenericError));
            _emailSenderService.SendAsync(Arg.Any<EmailMessage>())
                .Returns(Result.Success());

            // Act
            var actual = await _emailOperationService.ExecuteAsync(Request);

            // Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == CompressorCreatorErrors.GenericError.Code).Should().BeTrue();
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_ErrorWhenSendEmail_ShouldReturnResultError()
        {
            // Arrange & Act
            _compressorCreatorService.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<IExclusionRule>>())
                .Returns(Result.Success());
            _emailSenderService.SendAsync(Arg.Any<EmailMessage>())
                .Returns(Result.Failure(EmailSenderErrors.GenericError));

            // Act
            var actual = await _emailOperationService.ExecuteAsync(Request);

            // Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == EmailSenderErrors.GenericError.Code).Should().BeTrue();
        }
    }
}
