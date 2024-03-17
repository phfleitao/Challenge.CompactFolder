using CompactFolder.Domain.Operations;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using CompactFolder.Domain.Operations.FileShare;
using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations.FileShare
{
    public class FileShareOperationValidatorTests
    {
        private readonly FileShareOperationValidator _validator;

        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public TPath SharedPath { get; set; }

        public FileShareOperationValidatorTests()
        {
            _validator = new FileShareOperationValidator();

            OriginPath = @"C:\example\file.txt";
            OutputFileName = @"file.zip";
            ExclusionRules = new List<IExclusionRule> {
                new FileExtensionExclusionRule(new string[] { ".txt", ".jpg" }),
                new FileNameExclusionRule(new string[] { "excludedFile" }),
                new DirectoryNameExclusionRule(new string[] { "excludedDirectory" })
            };
            SharedPath = @"\\example";
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_ValidFileShareOperation_ShouldBeValid()
        {
            // Arrange
            var operation = new FileShareOperation(OriginPath, OutputFileName, ExclusionRules, SharedPath);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsSuccess.Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_WhenSharedPathIsNull_ShouldBeInvalid()
        {
            // Arrange
            var operation = new FileShareOperation(OriginPath, OutputFileName, ExclusionRules, null);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.Required.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_WhenSharedPathNotNetworkPath_ShouldBeInvalid()
        {
            // Arrange
            var operation = new FileShareOperation(OriginPath, OutputFileName, ExclusionRules, @"C:\folder");

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.NotNetworkPath.Code).Should().BeTrue();
        }
    }
}
