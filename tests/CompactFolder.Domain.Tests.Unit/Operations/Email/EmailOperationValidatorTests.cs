using CompactFolder.Domain.Common;
using CompactFolder.Domain.Operations;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.Operations.Email;
using CompactFolder.Domain.Operations.ExclusionRules;
using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations.Email
{
    public class EmailOperationValidatorTests
    {
        private readonly EmailOperationValidator _validator;

        public TPath OriginPath { get; set; }
        public TPath OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public TEmail EmailTo { get; set; }

        public EmailOperationValidatorTests()
        {
            _validator = new EmailOperationValidator();

            OriginPath = @"C:\example\file.txt";
            OutputFileName = @"file.zip";
            ExclusionRules = new List<IExclusionRule> {
                new FileExtensionExclusionRule(new string[] { ".txt", ".jpg" }),
                new FileNameExclusionRule(new string[] { "excludedFile" }),
                new DirectoryNameExclusionRule(new string[] { "excludedDirectory" })
            };
            EmailTo = @"pedro@example.com";
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_ValidEmailOperation_ShouldBeValid() 
        { 
            // Arrange
            var emailOperation = new EmailOperation(OriginPath, OutputFileName, ExclusionRules, EmailTo);

            // Act
            var actual = await _validator.ValidateAsync(emailOperation);

            //Assert
            actual.IsSuccess.Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public async Task ValidateAsync_WhenEmailToIsNull_ShouldBeInvalid()
        {
            // Arrange
            var emailOperation = new EmailOperation(OriginPath, OutputFileName, ExclusionRules, null);

            // Act
            var actual = await _validator.ValidateAsync(emailOperation);

            //Assert
            actual.IsFailure.Should().BeTrue();
            actual.Errors.Should().HaveCountGreaterThan(0);
            actual.Errors.Any(e => e.Code == OperationErrors.Required.Code).Should().BeTrue();
        }
    }
}
