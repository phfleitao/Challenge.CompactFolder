using CompactFolder.Domain.Operations.ExclusionRules;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations.ExclusionRules
{
    public class FileNameExclusionRuleTests
    {
        [Trait("Unit.Domain", "Operations")]
        [Theory]
        [InlineData(@"file.txt")]
        [InlineData(@"file.doc")]
        [InlineData(@"file.xml")]
        [InlineData(@"C:\file1")]
        [InlineData(@"C:\path\file2")]
        [InlineData(@"/file3")]
        [InlineData(@"/path/file4")]
        public void IsExcluded_ExcludedFileNames_ShouldReturnTrue(string pathOrFile)
        {
            // Arrange
            var excludedFileNames = new List<string> { "file", "file1", "file2", "file3", "file4" };
            var exclusionRule = new FileNameExclusionRule(excludedFileNames);

            // Act
            var actual = exclusionRule.IsExcluded(pathOrFile);

            // Assert
            actual.Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Theory]
        [InlineData(@"")]
        [InlineData(@"file.jpg")]
        [InlineData(@"file.png")]
        [InlineData(@"file.xlsx")]
        [InlineData(@"C:\file.jpg")]
        [InlineData(@"C:\file.png")]
        [InlineData(@"C:\path\file.xlsx")]
        [InlineData(@"/file.jpg")]
        [InlineData(@"/file.png")]
        [InlineData(@"/path/file.xlsx")]
        [InlineData(@"\\path\file.png")]
        [InlineData(@"\\path\file5")]
        public void IsExcluded_NonExcludedFileNames_ShouldReturnFalse(string pathOrFile)
        {
            // Arrange
            var excludedFileNames = new List<string> { "fileZZ", "fileYY", "filePP" };
            var exclusionRule = new FileNameExclusionRule(excludedFileNames);

            // Act
            var actual = exclusionRule.IsExcluded(pathOrFile);

            // Assert
            actual.Should().BeFalse();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void IsExcluded_NullExcludedFileNames_ShouldReturnFalse()
        {
            // Arrange
            var exclusionRule = new FileNameExclusionRule(null);

            // Act
            var actual = exclusionRule.IsExcluded("file.txt");

            // Assert
            actual.Should().BeFalse();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void IsExcluded_EmptyExcludedFileNames_ShouldReturnFalse()
        {
            // Arrange
            var exclusionRule = new FileNameExclusionRule(Enumerable.Empty<string>());

            // Act
            var actual = exclusionRule.IsExcluded("file.txt");

            // Assert
            actual.Should().BeFalse();
        }
    }
}
