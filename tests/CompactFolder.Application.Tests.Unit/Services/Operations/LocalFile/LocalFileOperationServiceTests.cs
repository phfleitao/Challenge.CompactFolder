using CompactFolder.Application.Services.CompressorService;
using CompactFolder.Application.Services.CompressorService.Contracts;
using CompactFolder.Application.Services.FileService;
using CompactFolder.Application.Services.FileServices.Contracts;
using CompactFolder.Application.Services.NetworkService;
using CompactFolder.Application.Services.Operations.LocalFile;
using CompactFolder.Application.Services.Operations.LocalFile.Contracts;
using CompactFolder.Application.Services.Operations.LocalFile.Models;
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

namespace CompactFolder.Application.Tests.Unit.Services.Operations.LocalFile
{
    public class LocalFileOperationServiceTests
    {
        private ILogger<LocalFileOperationService> _logger;
        private ICompressorCreator _compressorCreatorService;
        private IFileMover _fileMoverService;

        private ILocalFileOperationService _localFileOperationService;
        public LocalFileOperationRequest Request { get; set; }

        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public TPath DestinationPath { get; set; }

        public LocalFileOperationServiceTests()
        {
            _logger = Substitute.For<ILogger<LocalFileOperationService>>();

            _compressorCreatorService = Substitute.For<ICompressorCreator>();
            _fileMoverService = Substitute.For<IFileMover>();

            _localFileOperationService = new LocalFileOperationService(_logger, _compressorCreatorService, _fileMoverService);

            OriginPath = @"C:\example\file.txt";
            OutputFileName = @"file.zip";
            ExclusionRules = new List<IExclusionRule> {
                new FileExtensionExclusionRule(new string[] { ".txt", ".jpg" }),
                new FileNameExclusionRule(new string[] { "excludedFile" }),
                new DirectoryNameExclusionRule(new string[] { "excludedDirectory" })
            };
            DestinationPath = @"C:\example";

            Request = new LocalFileOperationRequest()
            {
                OriginPath = OriginPath.Path,
                OutputFileName = OutputFileName.Path,
                ExclusionRules = ExclusionRules,
                DestinationPath = DestinationPath.Path
            };
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_ValidLocalFileOperation_ShouldCompactAndMoveFile()
        {
            // Arrange          
            _compressorCreatorService.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<IExclusionRule>>())
                .Returns(Result.Success());
            _fileMoverService.Move(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Result.Success());

            // Act
            var response = await _localFileOperationService.ExecuteAsync(Request);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Errors.Should().HaveCount(0);
            response.Value.DestinationFullPath.Should().Contain(Path.Combine(Request.DestinationPath, Request.OutputFileName));
            _compressorCreatorService.Received().Create(
                Arg.Is(response.Value.OriginPath),
                Arg.Is(response.Value.CompressionPath),
                Arg.Is(response.Value.ExclusionRules));
            _fileMoverService.Received().Move(
                Arg.Is(response.Value.CompressionPath),
                Arg.Is(response.Value.DestinationFullPath));
        }
        
        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_InvalidDestinationPathNull_ShouldThrowException()
        {
            // Arrange & Act       
            var request = new LocalFileOperationRequest()
            {
                OriginPath = OriginPath.Path,
                OutputFileName = OutputFileName.Path,
                ExclusionRules = ExclusionRules,
                DestinationPath = null
            };

            // Act
            Func<Task> action = async () => await _localFileOperationService.ExecuteAsync(request);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        
        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_InvalidModelValidation_ShouldReturnResultError()
        {
            // Arrange & Act
            var request = new LocalFileOperationRequest()
            {
                OriginPath = OriginPath.Path,
                OutputFileName = "file.txt",
                ExclusionRules = ExclusionRules,
                DestinationPath = DestinationPath.Path
            };

            // Act
            var actual = await _localFileOperationService.ExecuteAsync(request);

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
            _fileMoverService.Move(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Result.Success());

            // Act
            var actual = await _localFileOperationService.ExecuteAsync(Request);

            // Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == CompressorCreatorErrors.GenericError.Code).Should().BeTrue();
        }

        [Trait("Unit.Application.Services", "Operations")]
        [Fact]
        public async Task ExecuteAsync_ErrorWhenMoveFileToFinalDestination_ShouldReturnResultError()
        {
            // Arrange & Act
            _compressorCreatorService.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<IExclusionRule>>())
                .Returns(Result.Success());
            _fileMoverService.Move(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Result.Failure(FileMoverErrors.GenericError));

            // Act
            var actual = await _localFileOperationService.ExecuteAsync(Request);

            // Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == FileMoverErrors.GenericError.Code).Should().BeTrue();
        }
    }
}
