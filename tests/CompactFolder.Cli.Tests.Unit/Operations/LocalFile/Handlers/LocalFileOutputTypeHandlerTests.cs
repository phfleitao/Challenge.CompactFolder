using CompactFolder.Application.Services.Operations.LocalFile.Contracts;
using CompactFolder.Application.Services.Operations.LocalFile.Models;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Operations.LocalFile.Handlers;
using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Common;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Cli.Tests.Unit.Operations.LocalFile.Handlers
{
    public class LocalFileOutputTypeHandlerTests
    {
        private readonly ILocalFileOperationService _localFileOperationService;
        public Result<LocalFileOperationResponse> SuccessResult { get; set; }
        public LocalFileOutputTypeHandler Handler { get; set; }
        public LocalFileOutputTypeHandlerTests()
        {
            _localFileOperationService = Substitute.For<ILocalFileOperationService>();
            SuccessResult = Result<LocalFileOperationResponse>.Success(new LocalFileOperationResponse());
            Handler = new LocalFileOutputTypeHandler(_localFileOperationService);
        }

        [Trait("Unit.Cli", "Operations")]
        [Fact]
        public async Task Handle_ValidOptions_ShouldReturnCorrectResult()
        {
            // Arrange
            _localFileOperationService.ExecuteAsync(Arg.Any<LocalFileOperationRequest>())
                .Returns(SuccessResult);
            var options = new Options { OriginPath = "origin", DestinationPath = "destination" };

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
            var options = new Options { OriginPath = "origin", DestinationPath = null };

            // Act
            Func<Task> action = async () => await Handler.Handle(options);

            // Assert
            action.Should().ThrowAsync<InvalidArgsOptionsException>()
                .WithMessage("Invalid argument *","Invalid argument ");
        }
    }
}
