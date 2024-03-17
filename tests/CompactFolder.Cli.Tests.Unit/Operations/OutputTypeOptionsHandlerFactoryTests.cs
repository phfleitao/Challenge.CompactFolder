using CompactFolder.Application.Services.Operations.Email.Contracts;
using CompactFolder.Application.Services.Operations.FileShare.Contracts;
using CompactFolder.Application.Services.Operations.LocalFile.Contracts;
using CompactFolder.Cli.Operations;
using CompactFolder.Cli.Operations.Email.Contracts;
using CompactFolder.Cli.Operations.Email.Handlers;
using CompactFolder.Cli.Operations.FileShare.Contracts;
using CompactFolder.Cli.Operations.FileShare.Handlers;
using CompactFolder.Cli.Operations.LocalFile.Contracts;
using CompactFolder.Cli.Operations.LocalFile.Handlers;
using FluentAssertions;
using NSubstitute;
using System;
using Xunit;

namespace CompactFolder.Cli.Tests.Unit.Operations
{
    public class OutputTypeOptionsHandlerFactoryTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILocalFileOperationService _localFileOperationService;
        private readonly IFileShareOperationService _fileShareOperationService;
        private readonly IEmailOperationService _emailOperationService;

        public OutputTypeOptionsHandlerFactoryTests()
        {
            _serviceProvider = Substitute.For<IServiceProvider>();
            _localFileOperationService = Substitute.For<ILocalFileOperationService>();
            _fileShareOperationService = Substitute.For<IFileShareOperationService>();
            _emailOperationService = Substitute.For<IEmailOperationService>();
        }

        [Trait("Unit.Cli", "Operations")]
        [Fact]
        public void Create_GivenValidOutputType_ShouldReturnCorrectHandlerType()
        {
            // Arrange
            _serviceProvider.GetService(typeof(ILocalFileOutputTypeHandler))
                .Returns(new LocalFileOutputTypeHandler(_localFileOperationService));
            _serviceProvider.GetService(typeof(IFileShareOutputTypeHandler))
                .Returns(new FileShareOutputTypeHandler(_fileShareOperationService));
            _serviceProvider.GetService(typeof(IEmailOutputTypeHandler))
                .Returns(new EmailOutputTypeHandler(_emailOperationService));

            var factory = new OutputTypeHandlerFactory(_serviceProvider);

            // Act
            var localFileHandler = factory.Create("LocalFile");
            var fileShareHandler = factory.Create("FileShare");
            var emailHandler = factory.Create("Email");

            // Assert
            localFileHandler.Should().BeOfType<LocalFileOutputTypeHandler>();
            fileShareHandler.Should().BeOfType<FileShareOutputTypeHandler>();
            emailHandler.Should().BeOfType<EmailOutputTypeHandler>();
        }

        [Trait("Unit.Cli", "Operations")]
        [Fact]
        public void Create_GivenInvalidOutputType_ShouldThrowExceptionForInvalidOutputType()
        {
            // Arrange
            var serviceProvider = Substitute.For<IServiceProvider>();
            var factory = new OutputTypeHandlerFactory(serviceProvider);

            // Act
            Action action = () => factory.Create("InvalidOutputType");

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage("Invalid output type: InvalidOutputType");
        }

        [Trait("Unit.Cli", "Operations")]
        [Fact]
        public void Create_GivenUnsupportedOutputType_ShouldThrowExceptionForUnsupportedOutputType()
        {
            // Arrange
            var serviceProvider = Substitute.For<IServiceProvider>();
            var factory = new OutputTypeHandlerFactory(serviceProvider);

            // Act
            Action action = () => factory.Create("3");

            // Assert
            action.Should().Throw<NotSupportedException>().WithMessage("Unsupported output type: 3");
        }        
    }
}
