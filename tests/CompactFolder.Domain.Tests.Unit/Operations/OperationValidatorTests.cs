using CompactFolder.Domain.Operations;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.ExclusionRules;
using CompactFolder.Domain.Tests.Unit.TestUtils.ConcreteObjects;
using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations
{
    public partial class OperationValidatorTests
    {
        private readonly ConcreteObjectOperationValidator _validator;

        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }

        public OperationValidatorTests()
        {
            _validator = new ConcreteObjectOperationValidator();

            OriginPath = @"C:\example\file.txt";
            OutputFileName = @"file.zip";
            ExclusionRules = new List<IExclusionRule> {
                new FileExtensionExclusionRule(new string[] { ".txt", ".jpg" }),
                new FileNameExclusionRule(new string[] { "excludedFile" }),
                new DirectoryNameExclusionRule(new string[] { "excludedDirectory" })
            };
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_NullOriginPath_ShouldReturnError()
        {
            // Arrange
            var operation = new ConcreteObjectOperation(null, OutputFileName, ExclusionRules);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.Required.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_NotFullPathOrDirectoryOriginPath_ShouldReturnError()
        {
            // Arrange
            var operation = new ConcreteObjectOperation(@"folder", OutputFileName, ExclusionRules);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.NotFullPathOrDirectory.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_NullOutputFileName_ShouldReturnError()
        {
            // Arrange
            var operation = new ConcreteObjectOperation(OriginPath, null, ExclusionRules);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.Required.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_NotOnlyFileOutputFileName_ShouldReturnError()
        {
            // Arrange
            var operation = new ConcreteObjectOperation(OriginPath, @"C:\folder\file.zip", ExclusionRules);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.NotOnlyFileName.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_NullCompressionPath_ShouldReturnError()
        {
            // Arrange
            var operation = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules);
            operation.SetCompressionPath(null);

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.Required.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_CompressionPathWithoutFullPath_ShouldReturnError()
        {
            // Arrange          
            var operation = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules);
            operation.SetCompressionPath(@"folder");

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.NotFullPath.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_CompressionPathAsNetworkPath_ShouldReturnError()
        {
            // Arrange
            var operation = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules);
            operation.SetCompressionPath(@"\\tempfolder\subfolder");

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.IsNetworkPath.Code).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_CompressionPathWithoutZipExtension_ShouldReturnError()
        {
            // Arrange
            var operation = new ConcreteObjectOperation(OriginPath, OutputFileName, ExclusionRules);
            operation.SetCompressionPath(@"C:\folder\file.jpg");

            // Act
            var actual = await _validator.ValidateAsync(operation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.NotZipFileExtension.Code).Should().BeTrue();
        }
    }
}
