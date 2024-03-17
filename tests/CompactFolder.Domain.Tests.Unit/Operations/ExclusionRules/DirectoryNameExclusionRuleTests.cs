using CompactFolder.Domain.Operations.ExclusionRules;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Operations.ExclusionRules
{
    public class DirectoryNameExclusionRuleTests
    {
        [Trait("Unit.Domain", "Operations")]
        [Theory]
        [InlineData(@"C:\excluded\file.txt")]
        [InlineData(@"C:\excluded\path\file.doc")]
        [InlineData(@"/excluded/file.xml")]
        [InlineData(@"/excludedNew/path/file.xlsx")]
        [InlineData(@"excluded")]
        [InlineData(@"path")]
        public void IsExcluded_ExcludedDirectoryNames_ShouldReturnTrue(string pathOrFile)
        {
            // Arrange
            var excludedDirectoryNames = new List<string> { "excluded", "path" };
            var exclusionRule = new DirectoryNameExclusionRule(excludedDirectoryNames);

            // Act
            var actual = exclusionRule.IsExcluded(pathOrFile);

            // Assert
            actual.Should().BeTrue();
        }

        [Trait("Unit.Domain", "Operations")]
        [Theory]
        [InlineData(@"")]
        [InlineData(@"C:\path\file.jpg")]
        [InlineData(@"C:\file.png")]
        [InlineData(@"C:\path\file.xlsx")]
        [InlineData(@"/file.jpg")]
        [InlineData(@"/file.png")]
        [InlineData(@"/path/file.xlsx")]
        [InlineData(@"\\path\file.png")]
        [InlineData(@"\\path\file5")]
        [InlineData(@"excludedtest")]
        [InlineData(@"pathtest")]
        public void IsExcluded_NonExcludedDirectoryNames_ShouldReturnFalse(string pathOrFile)
        {
            // Arrange
            var excludedDirectoryNames = new List<string> { "exclude", "excludedPath" };
            var exclusionRule = new DirectoryNameExclusionRule(excludedDirectoryNames);

            // Act
            var actual = exclusionRule.IsExcluded(pathOrFile);

            // Assert
            actual.Should().BeFalse();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void IsExcluded_NullExcludedDirectoryNames_ShouldReturnFalse()
        {
            // Arrange
            var exclusionRule = new DirectoryNameExclusionRule(null);

            // Act
            var actual = exclusionRule.IsExcluded(@"C:\excluded\file.txt");

            // Assert
            actual.Should().BeFalse();
        }

        [Trait("Unit.Domain", "Operations")]
        [Fact]
        public void IsExcluded_EmptyExcludedDirectoryNames_ShouldReturnFalse()
        {
            // Arrange
            var exclusionRule = new DirectoryNameExclusionRule(Enumerable.Empty<string>());

            // Act
            var actual = exclusionRule.IsExcluded(@"C:\excluded\file.txt");

            // Assert
            actual.Should().BeFalse();
        }
    }
}
