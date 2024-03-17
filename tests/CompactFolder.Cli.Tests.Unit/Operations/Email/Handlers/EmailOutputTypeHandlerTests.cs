using CompactFolder.Application.Services.Operations.Email.Contracts;
using CompactFolder.Application.Services.Operations.Email.Models;
using CompactFolder.Cli.Exceptions;
using CompactFolder.Cli.Operations.Email.Handlers;
using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Common;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CompactFolder.Cli.Tests.Unit.Operations.Email.Handlers
{
    public class EmailOutputTypeHandlerTests
    {
        private readonly IEmailOperationService _emailOperationService;
        public Result<EmailOperationResponse> SuccessResult { get; set; }
        public EmailOutputTypeHandler Handler { get; set; }
        public EmailOutputTypeHandlerTests()
        {
            _emailOperationService = Substitute.For<IEmailOperationService>();
            SuccessResult = Result<EmailOperationResponse>.Success(new EmailOperationResponse());
            Handler = new EmailOutputTypeHandler(_emailOperationService);
        }

        [Trait("Unit.Cli", "Operations")]
        [Fact]
        public async Task Handle_ValidOptions_ShouldReturnCorrectResult()
        {
            // Arrange
            _emailOperationService.ExecuteAsync(Arg.Any<EmailOperationRequest>())
                .Returns(SuccessResult);
            var options = new Options { OriginPath = "origin", EmailTo = "email@email.com" };

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
            var options = new Options { OriginPath = "origin", EmailTo = null };

            // Act
            Func<Task> action = async () => await Handler.Handle(options);

            // Assert
            action.Should().ThrowAsync<InvalidArgsOptionsException>()
                .WithMessage("Invalid argument *", "Invalid argument ");
        }
    }
}
