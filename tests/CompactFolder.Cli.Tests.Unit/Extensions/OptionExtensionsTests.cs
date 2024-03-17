using CompactFolder.Cli.Extensions;
using CompactFolder.Cli.Operations.Models;
using FluentAssertions;
using System;
using Xunit;

namespace CompactFolder.Cli.Tests.Unit.Extensions
{
    public class OptionExtensionsTests
    {
        [Trait("Unit.Cli", "Extensions")]
        [Fact]
        public void GetAttributes_WithBasicAttributes_ShouldReturnCorrectAttribute()
        {
            // Arrange
            var options = new Options();
            var propertyName = nameof(Options.OriginPath);
            var expectedLongName = "inputPath";

            // Act
            var attribute = options.GetAttributes(propertyName);

            // Assert
            attribute.Should().NotBeNull();
            attribute.LongName.Should().Be(expectedLongName);
        }

        [Trait("Unit.Cli", "Extensions")]
        [Fact]
        public void GetAttributes_WithExtendedInterfacedAttributes_ShouldReturnCorrectAttribute()
        {
            // Arrange
            var options = new Options();
            var propertyName = nameof(Options.EmailTo);
            var expectedLongName = "emailTo";

            // Act
            var attribute = options.GetAttributes(propertyName);

            // Assert
            attribute.Should().NotBeNull();
            attribute.LongName.Should().Be(expectedLongName);
        }

        [Trait("Unit.Cli", "Extensions")]
        [Fact]
        public void GetAttributes_WithInvalidPropertyName_ShouldThrowException()
        {
            // Arrange
            var options = new Options();

            // Act
            Action act = () => options.GetAttributes("InvalidPropertyName");

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Property InvalidPropertyName not found on type Options");
        }
    }
}
