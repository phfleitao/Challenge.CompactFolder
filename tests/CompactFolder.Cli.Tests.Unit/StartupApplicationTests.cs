using CommandLine;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Operations.Contracts;
using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Cli.Tests.Unit
{
    public class StartupApplicationTests
    {
        private readonly IHost _host;
        private readonly IOutputTypeHandlerFactory _outputTypeHandlerFactory;
        private readonly ILogger<StartupApplication> _logger;
        private readonly IOutputTypeHandler _handler;
        private readonly StartupApplication _startupApplication;

        public StartupApplicationTests()
        {
            _host = Substitute.For<IHost>();
            _outputTypeHandlerFactory = Substitute.For<IOutputTypeHandlerFactory>();
            _logger = Substitute.For<ILogger<StartupApplication>>();
            _handler = Substitute.For<IOutputTypeHandler>();
            _startupApplication = Substitute.For<StartupApplication>(_host);
            
            _host.Services.GetService<ILogger<StartupApplication>>().Returns(_logger);
        }

        [Trait("Unit.Cli", "Core")]
        [Fact]
        public async Task RunWithParser_ShouldReturnSuccess_WhenNoExceptionOccurs()
        {
            // Arrange
            var args = @"-i C:\Tests -o file.zip -t localFile".SplitArgs();
            _host.Services.GetService<IOutputTypeHandlerFactory>()
                .Returns(_outputTypeHandlerFactory);
            _outputTypeHandlerFactory.Create(Arg.Any<string>()).Returns(_handler);
            _handler.Handle(Arg.Any<Options>()).Returns(Result.Success());

            // Act
            var result = await _startupApplication.RunAsync(args);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Trait("Unit.Cli", "Core")]
        [Fact]
        public async Task RunAsync_WhenThrowsArgumentException_ShouldReturnFailureResult()
        {
            // Arrange
            var args = @"-i C:\Tests -o file.zip -t localFile".SplitArgs();
            _host.Services.GetService(typeof(IOutputTypeHandlerFactory))
                .Returns(x => throw new ArgumentException("Argument Error"));

            // Act
            var result = await _startupApplication.RunAsync(args);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.FirstError.Code.Should().Be("Cli.Argument");
        }

        [Trait("Unit.Cli", "Core")]
        [Fact]
        public async Task RunAsync_WhenThrowsInvalidArgsOptionsException_ShouldReturnFailureResult()
        {
            // Arrange
            var args = @"-i C:\Tests -o file.zip -t localFile".SplitArgs();
            _host.Services.GetService(typeof(IOutputTypeHandlerFactory))
                .Returns(x => throw new InvalidArgsOptionsException("InvalidArgsOptions Error"));

            // Act
            var result = await _startupApplication.RunAsync(args);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.FirstError.Code.Should().Be("Cli.Argument");
        }

        [Trait("Unit.Cli", "Core")]
        [Fact]
        public async Task RunAsync_WhenThrowsGeneralException_ShouldReturnFailureResult()
        {
            // Arrange
            var args = @"-i C:\Tests -o file.zip -t localFile".SplitArgs();
            _host.Services.GetService(typeof(IOutputTypeHandlerFactory))
                .Returns(x => throw new Exception("Generic Error"));

            // Act
            var result = await _startupApplication.RunAsync(args);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.FirstError.Code.Should().Be("Cli.General");
        }
    }
}
