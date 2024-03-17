using CompactFolder.Application.Services.Operations.FileShare.Contracts;
using CompactFolder.Application.Services.Operations.FileShare.Models;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Operations.FileShare.Handlers;
using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Common;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Cli.Tests.Unit.Operations.FileShare.Handlers
{
    public class FileShareOutputTypeHandlerTests
    {
        private readonly IFileShareOperationService _emailOperationService;
        public Result<FileShareOperationResponse> SuccessResult { get; set; }
        public FileShareOutputTypeHandler Handler { get; set; }
        public FileShareOutputTypeHandlerTests()
        {
            _emailOperationService = Substitute.For<IFileShareOperationService>();
            SuccessResult = Result<FileShareOperationResponse>.Success(new FileShareOperationResponse());
            Handler = new FileShareOutputTypeHandler(_emailOperationService);
        }

        [Trait("Unit.Cli", "Operations")]
        [Fact]
        public async Task Handle_ValidOptions_ShouldReturnCorrectResult()
        {
            // Arrange
            _emailOperationService.ExecuteAsync(Arg.Any<FileShareOperationRequest>())
                .Returns(SuccessResult);
            var options = new Options { OriginPath = "origin", SharedPath = "sharedPath" };

            // Act
            var result = await Handler.Handle(options);

            // Assert
            result.Should().BeEquivalentTo(SuccessResult);
        }

        [Trait("Unit.Cli", "Operations")]
        [Fact]
        public void Handle_InvalidOptions_ShouldThrowExceptionForInvalidOptions()
        {
            // Arrange
            var options = new Options { OriginPath = "origin", SharedPath = null };

            // Act
            Func<Task> action = async () => await Handler.Handle(options);

            // Assert
            action.Should().ThrowAsync<InvalidArgsOptionsException>()
                .WithMessage("Invalid argument *", "Invalid argument ");
        }
    }
}
