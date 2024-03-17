using CompactFolder.Cli.Exceptions;
using FluentAssertions;
using Xunit;

namespace CompactFolder.Cli.Tests.Unit.Exceptions
{
    public class InvalidArgsOptionsExceptionTests
    {
        [Trait("Unit.Cli", "Exceptions")]
        [Fact]
        public void Create_InvalidArgsOptionsException_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var optionName = "TestOption";

            // Act
            var exception = new InvalidArgsOptionsException(optionName);

            // Assert
            exception.OptionName.Should().Be(optionName);
            exception.Message.Should().Be($"Invalid argument {optionName}");
        }
    }
}
