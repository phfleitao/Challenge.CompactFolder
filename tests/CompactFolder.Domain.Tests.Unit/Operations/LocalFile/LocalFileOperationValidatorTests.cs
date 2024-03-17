using CompactFolder.Domain.Operations;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using CompactFolder.Domain.Operations.LocalFile;
using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations.LocalFile
{
    public class LocalFileOperationValidatorTests
    {
        private readonly LocalFileOperationValidator _validator;

        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public TPath DestinationPath { get; set; }

        public LocalFileOperationValidatorTests()
        {
            _validator = new LocalFileOperationValidator();

            OriginPath = @"C:\example\file.txt";
            OutputFileName = @"file.zip";
            ExclusionRules = new List<IExclusionRule> {
                new FileExtensionExclusionRule(new string[] { ".txt", ".jpg" }),
                new FileNameExclusionRule(new string[] { "excludedFile" }),
                new DirectoryNameExclusionRule(new string[] { "excludedDirectory" })
            };
            DestinationPath = @"C:\example";
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_ValidLocalFileOperation_ShouldBeValid()
        {
            // Arrange
            var operation = new LocalFileOperation(OriginPath, OutputFileName, ExclusionRules, DestinationPath);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsSuccess.Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_WhenDestinationPathIsNull_ShouldBeInvalid()
        {
            // Arrange
            var operation = new LocalFileOperation(OriginPath, OutputFileName, ExclusionRules, null);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.Required.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_WhenDestinationPathIsNotRootedDirectory_ShouldBeInvalid()
        {
            // Arrange
            var operation = new LocalFileOperation(OriginPath, OutputFileName, ExclusionRules, @"folder/subfolder");

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.NotRootedDirectoryPath.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_WhenDestinationPathIsNetworkPath_ShouldBeInvalid()
        {
            // Arrange
            var operation = new LocalFileOperation(OriginPath, OutputFileName, ExclusionRules, @"\\folder\subfolder");

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.IsNetworkPath.Code).Should().BeTrue();
        }
    }
}
