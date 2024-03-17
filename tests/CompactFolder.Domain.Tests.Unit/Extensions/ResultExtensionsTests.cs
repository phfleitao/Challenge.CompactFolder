using CompactFolder.Domain.Common;
using CompactFolder.Domain.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Extensions
{
    public class ResultExtensionsTests
    {
        [Trait("Unit.Domain", "Extensions")]
        [Fact]
        public void AsFailureResult_WhenGivenFailureBaseResult_ShouldReturnConvertedResult()
        {
            //Arrange
            var failureResult = Result.Failure(new Error("ErrorCode", "ErrorDescription"));

            // Act
            var convertedResult = failureResult.AsFailureResult<string>();

            // Assert
            convertedResult.IsSuccess.Should().BeFalse();
            convertedResult.Errors.Should().BeEquivalentTo(failureResult.Errors);
            convertedResult.Value.Should().Be(default);
            convertedResult.Should().BeOfType<Result<string>>();
        }

        [Trait("Unit.Domain", "Extensions")]
        [Fact]
        public void AsFailureResult_WhenGivenSuccessulBaseResult_ShouldThrowError()
        {
            //Arrange
            var successfulResult = Result.Success();

            // Act
            Action action = () => successfulResult.AsFailureResult<string>();
            
            // Assert
            action.Should().Throw<InvalidOperationException>().WithMessage("Cannot convert a successful Result to Result<T> without a value.");
        }        
    }
}
