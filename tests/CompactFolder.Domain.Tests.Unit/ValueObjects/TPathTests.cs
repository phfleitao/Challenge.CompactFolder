using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.ValueObjects
{
    public class TPathTests
    {
        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example")]
        [InlineData(@"C:\example\")]
        [InlineData(@"C:\example\file.txt")]
        [InlineData(@"\\server\share")]
        [InlineData(@"\\server\share\")]
        [InlineData(@"\\server\share\file.txt")]
        [InlineData(@"/unix/valid/path")]
        [InlineData(@"/unix/valid/path/")]
        [InlineData(@"/unix/path/file.txt")]
        public void Create_ValidPath_ShouldReturnSuccess(string validPath)
        {
            // Act
            var path = TPath.Create(validPath);

            // Assert
            path.Should().NotBeNull();
            path.Path.Should().Be(validPath);
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example", "file.txt")]
        [InlineData(@"C:\example\", "file.txt")]
        [InlineData(@"\\server\share", "file.txt")]
        [InlineData(@"\\server\share\", "file.txt")]
        [InlineData(@"/unix/valid/path", "file.txt")]
        [InlineData(@"/unix/valid/path/", "file.txt")]
        public void Create_ValidPathAndFileName_ShouldReturnSuccess(string path, string fileName)
        {
            // Arrange & Act
            var actual = TPath.Create(path, fileName);

            // Assert
            actual.Should().NotBeNull();
            actual.Path.Should().Be(Path.Combine(path, fileName));
            actual.FileName.Should().Be(fileName);
            actual.FileExtension.Should().Be(Path.GetExtension(fileName));            
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("path/invalid\npath")]
        [InlineData("invalid\tfile.txt")]
        [InlineData(@"test/with|pipe")]
        [InlineData(@"test/with|pipe/file.txt")]
        [InlineData(@"/test/with|pipe/file.txt")]
        [InlineData(@"C:\test\with|pipe\file.txt")]
        public void Create_InvalidPath_ShouldThrowArgumentException(string invalidPath)
        {
            // Act & Assert
            Action act = () => TPath.Create(invalidPath);
            act.Should().Throw<ArgumentException>();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example\file.txt", @"C:\example\file.txt")]
        [InlineData(@"/unix/path/file.txt", @"/unix/path/file.txt")]
        public void GetEqualityComponents_SamePaths_ShouldReturnEqual(string firstPath, string secondPath)
        {
            // Arrange
            var firstTPath = TPath.Create(firstPath);
            var secondTPath = TPath.Create(secondPath);

            // Act & Assert
            firstTPath.Should().Be(secondTPath);
            firstTPath.GetHashCode().Should().Be(secondTPath.GetHashCode());
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example\file.txt", @"\\server\share\file.txt")]
        [InlineData(@"C:\example\file.txt", @"C:\different\file.txt")]
        [InlineData(@"/unix/path/file.txt", @"/unix/another/file.txt")]
        [InlineData(@"C:\example\file.txt", @"C:\different\file.txt ")]
        public void GetEqualityComponents_DifferentPaths_ShouldReturnNotEqual(string firstPath, string secondPath)
        {
            // Arrange
            var firstTPath = TPath.Create(firstPath);
            var secondTPath = TPath.Create(secondPath);

            // Act & Assert
            firstTPath.Should().NotBe(secondTPath);
            firstTPath.GetHashCode().Should().NotBe(secondTPath.GetHashCode());
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example\file.txt")]
        [InlineData(@"/unix/path/file.txt")]
        [InlineData(@"\\nework\path\file.txt")]
        [InlineData(@"file.txt")]
        public void IsFile_WithFilePath_ShouldReturnTrue(string filePath)
        {
            // Arrange
            var path = TPath.Create(filePath);

            // Act
            bool isFile = path.IsFile();

            // Assert
            isFile.Should().BeTrue();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example\file")]
        [InlineData(@"C:\example\")]
        [InlineData(@"/unix/path/file")]
        [InlineData(@"/unix/path/")]
        [InlineData(@"\\nework\path\file")]
        [InlineData(@"\\nework\path\")]
        [InlineData(@"file")]
        public void IsFile_WithFolderPath_ShouldReturnFalse(string folderPath)
        {
            // Arrange
            var path = TPath.Create(folderPath);

            // Act
            bool isFile = path.IsFile();

            // Assert
            isFile.Should().BeFalse();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"file.txt")]
        [InlineData(@"file2.png")]
        public void IsOnlyFile_WithJustFileAndExtension_ShouldReturnTrue(string filePath)
        {
            // Arrange
            var path = TPath.Create(filePath);

            // Act
            bool isFile = path.IsOnlyFile();

            // Assert
            isFile.Should().BeTrue();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\file.txt")]
        [InlineData(@"C:\example")]
        [InlineData(@"C:\example\")]
        [InlineData(@"C:\example\file.txt")]
        [InlineData(@"/file.txt")]
        [InlineData(@"/unix")]
        [InlineData(@"/unix/")]
        [InlineData(@"/unix/file.txt")]
        [InlineData(@"\\file.txt")]
        [InlineData(@"\\example")]
        [InlineData(@"\\example\")]
        [InlineData(@"\\example\file.txt")]
        public void IsOnlyFile_WithFolderPath_ShouldReturnFalse(string filePath)
        {
            // Arrange
            var path = TPath.Create(filePath);

            // Act
            bool isFile = path.IsOnlyFile();

            // Assert
            isFile.Should().BeFalse();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example\file.txt")]
        [InlineData(@"C:\file.txt")]
        [InlineData(@"/unix/path/file.txt")]
        [InlineData(@"/file.txt")]
        [InlineData(@"\\server\share\file.txt")]
        [InlineData(@"\\file.txt")]
        public void IsFullPath_WithRootPathAndFile_ShouldReturnTrue(string fullPath)
        {
            // Arrange
            var path = TPath.Create(fullPath);

            // Act
            bool isFullPath = path.IsFullPath();

            // Assert
            isFullPath.Should().BeTrue();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example")]
        [InlineData(@"C:\example\")]
        [InlineData(@"/unix/path/")]
        [InlineData(@"\\server\share")]
        [InlineData(@"\\server\share\")]
        [InlineData(@"example/file.txt")]
        public void IsFullPath_WithNoRootPathAndFile_ShouldReturnFalse(string fullPath)
        {
            // Arrange
            var path = TPath.Create(fullPath);

            // Act
            bool isFullPath = path.IsFullPath();

            // Assert
            isFullPath.Should().BeFalse();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData("example")]
        [InlineData(@"example\path")]
        [InlineData(@"example\path\")]
        [InlineData(@"example\path\file.txt")]
        [InlineData(@"..\example\path")]
        [InlineData(@"..\example\path\file.txt")]
        [InlineData("unix/path")]
        [InlineData("unix/path/")]
        [InlineData("../unix/path")]
        [InlineData("../unix/path/")]
        [InlineData("../unix/path/file.txt")]
        public void IsRelativePath_WithNoRootPathAndNoFile_ShouldReturnTrue(string relativePath)
        {
            // Arrange
            var path = TPath.Create(relativePath);

            // Act
            bool isFullPath = path.IsRelativePath();

            // Assert
            isFullPath.Should().BeTrue();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"C:\example\path")]
        [InlineData(@"C:\example\path\")]
        [InlineData(@"C:\example\path\file.txt")]
        [InlineData("/unix/path")]
        [InlineData("/unix/path/")]
        [InlineData("/unix/path/file.txt")]
        public void IsRelativePath_WithNoRootPathAndNoFile_ShouldReturnFalse(string relativePath)
        {
            // Arrange
            var path = TPath.Create(relativePath);

            // Act
            bool isFullPath = path.IsRelativePath();

            // Assert
            isFullPath.Should().BeFalse();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData(@"\\server\share")]
        [InlineData(@"\\server\share\file.txt")]
        public void IsNetworkPath_WithNetworkPath_ShouldReturnTrue(string networkPath)
        {
            // Arrange
            var pathComponent = TPath.Create(networkPath);

            // Act
            bool isNetworkPath = pathComponent.IsNetworkPath();

            // Assert
            isNetworkPath.Should().BeTrue();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData("example")]
        [InlineData(@"example\path")]
        [InlineData(@"example\path\")]
        [InlineData(@"example\path\file.txt")]
        [InlineData(@"..\example\path")]
        [InlineData(@"..\example\path\file.txt")]
        [InlineData("unix/path")]
        [InlineData("unix/path/")]
        [InlineData("../unix/path")]
        [InlineData("../unix/path/")]
        [InlineData("../unix/path/file.txt")]
        public void IsNetworkPath_WithLocalPaths_ShouldReturnFalse(string networkPath)
        {
            // Arrange
            var pathComponent = TPath.Create(networkPath);

            // Act
            bool isNetworkPath = pathComponent.IsNetworkPath();

            // Assert
            isNetworkPath.Should().BeFalse();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Fact]
        public void ToString_WithValidPath_ShouldReturnPathAsString()
        {
            // Arrange
            var path = TPath.Create(@"C:\folder");

            // Act
            var pathString = path.ToString();

            // Assert
            pathString.Should().Be(@"C:\folder");
        }
    }
}
