using CompactFolder.Domain.Operations.ExclusionRules;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations.ExclusionRules
{
    public class FileExtensionExclusionRuleTests
    {
        [Trait("Unit.Domain", "Operations")]
        [Theory]
        [InlineData(@"file.txt")]
        [InlineData(@"file.jpg")]
        [InlineData(@"file.png")]
        [InlineData(@"C:\file.txt")]
        [InlineData(@"C:\file.jpg")]
        [InlineData(@"C:\path\file.png")]
        [InlineData(@"/file.txt")]
        [InlineData(@"/file.jpg")]
        [InlineData(@"/path/file.png")]
        [InlineData(@"\\path\file.png")]
        public void IsExcluded_ExcludedFileExtensions_ShouldReturnTrue(string pathOrFile)
        {
            // Arrange
            var excludedFileExtensions = new List<string> { ".txt", ".jpg", ".png" };
            var exclusionRule = new FileExtensionExclusionRule(excludedFileExtensions);

            // Act
            var actual = exclusionRule.IsExcluded(pathOrFile);

            //Assert
            actual.Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Theory]
        [InlineData(@"file.doc")]
        [InlineData(@"file.xml")]
        [InlineData(@"file.xlsx")]
        [InlineData(@"C:\file.zip")]
        [InlineData(@"C:\file.7z")]
        [InlineData(@"C:\path\file.docx")]
        [InlineData(@"/file.doc")]
        [InlineData(@"/file.bmp")]
        [InlineData(@"/path/file.xls")]
        [InlineData(@"\\path\file.cs")]
        public void IsExcluded_NonExcludedFileExtensions_ShouldReturnFalse(string pathOrFile)
        {
            // Arrange
            var excludedFileExtensions = new List<string> { ".txt", ".jpg", ".png" };
            var exclusionRule = new FileExtensionExclusionRule(excludedFileExtensions);

            // Act
            var actual = exclusionRule.IsExcluded(pathOrFile);

            //Assert
            actual.Should().BeFalse();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void IsExcluded_NullExcludedFileExtensions_ShouldReturnFalse()
        {
            // Arrange
            var exclusionRule = new FileExtensionExclusionRule(null);

            // Act
            var actual = exclusionRule.IsExcluded("file.txt");

            //Assert
            actual.Should().BeFalse();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void IsExcluded_EmptyExcludedFileExtensions_ShouldReturnFalse()
        {
            // Arrange
            var exclusionRule = new FileExtensionExclusionRule(Enumerable.Empty<string>());

            // Act
            var actual = exclusionRule.IsExcluded("file.txt");

            //Assert
            actual.Should().BeFalse();
        }
    }
}
