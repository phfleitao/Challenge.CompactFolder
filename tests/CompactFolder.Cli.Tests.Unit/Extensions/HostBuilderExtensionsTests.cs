using CompactFolder.Cli.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;

namespace CompactFolder.Cli.Tests.Unit.Extensions
{
    public class HostBuilderExtensionsTests
    {
        [Trait("Unit.Cli", "Extensions")]
        [Fact]
        public void AddConfiguration_ShouldNotThrowException()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            // Act
            Action act = () => hostBuilder.AddConfiguration();

            // Assert
            act.Should().NotThrow();
        }

        [Trait("Unit.Cli", "Extensions")]
        [Fact]
        public void AddServices_ShouldNotThrowException()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            // Act
            Action act = () => hostBuilder.AddServices();

            // Assert
            act.Should().NotThrow();
        }

        [Trait("Unit.Cli", "Extensions")]
        [Fact]
        public void AddLogging_ShouldNotThrowException()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            // Act
            Action act = () => hostBuilder.AddLogging();

            // Assert
            act.Should().NotThrow();
        }
    }
}
