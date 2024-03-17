using CompactFolder.Application.Services.CompressorService;
using CompactFolder.Application.Services.CompressorService.Contracts;
using CompactFolder.Application.Services.NetworkService;
using CompactFolder.Application.Services.NetworkService.Contracts;
using CompactFolder.Application.Services.Operations.FileShare;
using CompactFolder.Application.Services.Operations.FileShare.Contracts;
using CompactFolder.Application.Services.Operations.FileShare.Models;
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

namespace CompactFolder.Application.Tests.Unit.Services.Operations.FileShare
{
    public class FileShareOperationServiceTests
    {
        private ILogger<FileShareOperationService> _logger;
        private ICompressorCreator _compressorCreatorService;
        private INetworkFileSender _networkFileSender;

        private IFileShareOperationService _fileShareOperationService;
        public FileShareOperationRequest Request { get; set; }

        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public TPath SharedPath { get; set; }

        public FileShareOperationServiceTests()
        {
            _logger = Substitute.For<ILogger<FileShareOperationService>>();
            _compressorCreatorService = Substitute.For<ICompressorCreator>();
            _networkFileSender = Substitute.For<INetworkFileSender>();

            _fileShareOperationService = new FileShareOperationService(_logger, _compressorCreatorService, _networkFileSender);

            OriginPath = @"C:\example\file.txt";
            OutputFileName = @"file.zip";
            ExclusionRules = new List<IExclusionRule> {
                new FileExtensionExclusionRule(new string[] { ".txt", ".jpg" }),
                new FileNameExclusionRule(new string[] { "excludedFile" }),
                new DirectoryNameExclusionRule(new string[] { "excludedDirectory" })
            };
            SharedPath = @"\\example";

            Request = new FileShareOperationRequest()
            {
                OriginPath = OriginPath.Path,
                OutputFileName = OutputFileName.Path,
                ExclusionRules = ExclusionRules,
                SharedPath = SharedPath.Path
            };
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_ValidFileShareOperation_ShouldCompactAndSendFile()
        {
            // Arrange          
            _compressorCreatorService.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<IExclusionRule>>())
                .Returns(Result.Success());
            _networkFileSender.Send(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Result.Success());

            // Act
            var response = await _fileShareOperationService.ExecuteAsync(Request);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Errors.Should().HaveCount(0);
            response.Value.SharedFullPath.Should().Contain(Path.Combine(Request.SharedPath, Request.OutputFileName));
            _compressorCreatorService.Received().Create(
                Arg.Is(response.Value.OriginPath),
                Arg.Is(response.Value.CompressionPath),
                Arg.Is(response.Value.ExclusionRules));
            _networkFileSender.Received().Send(
                Arg.Is(response.Value.CompressionPath),
                Arg.Is(response.Value.SharedFullPath));
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_InvalidSharedPathNull_ShouldThrowException()
        {
            // Arrange & Act       
            var request = new FileShareOperationRequest()
            {
                OriginPath = OriginPath.Path,
                OutputFileName = OutputFileName.Path,
                ExclusionRules = ExclusionRules,
                SharedPath = null
            };

            // Act
            Func<Task> action = async () => await _fileShareOperationService.ExecuteAsync(request);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_InvalidModelValidation_ShouldReturnResultError()
        {
            // Arrange & Act
            var request = new FileShareOperationRequest()
            {
                OriginPath = OriginPath.Path,
                OutputFileName = "file.txt",
                ExclusionRules = ExclusionRules,
                SharedPath = SharedPath.Path
            };

            // Act
            var actual = await _fileShareOperationService.ExecuteAsync(request);

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
            _networkFileSender.Send(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Result.Success());

            // Act
            var actual = await _fileShareOperationService.ExecuteAsync(Request);

            // Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == CompressorCreatorErrors.GenericError.Code).Should().BeTrue();
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_ErrorWhenSendFileToFinalDestination_ShouldReturnResultError()
        {
            // Arrange & Act
            _compressorCreatorService.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<IExclusionRule>>())
                .Returns(Result.Success());
            _networkFileSender.Send(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Result.Failure(NetworkFileSenderErrors.GenericError));

            // Act
            var actual = await _fileShareOperationService.ExecuteAsync(Request);

            // Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == NetworkFileSenderErrors.GenericError.Code).Should().BeTrue();
        }
    }
}
