using CompactFolder.Domain.Common;
using CompactFolder.Domain.Tests.Unit.TestUtils.ConcreteObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Common
{
    public class ResultTests
    {
        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void Result_Success_ShouldReturnSuccessResult()
        {
            // Arrange
            // Act
            var result = Result.Success();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void Result_Failure_ShouldReturnFailureResultWithErrors()
        {
            // Arrange
            var errors = new List<Error> { new Error("ErrorCode", "Error Description") };

            // Act
            var result = Result.Failure(errors);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(errors);
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void Result_Failure_ShouldReturnFailureResultWithError()
        {
            // Arrange
            var error = new Error("ErrorCode", "Error Description");

            // Act
            var result = Result.Failure(error);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.Should().Be(error);
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void Result_WhenCreateSuccessResultWithErrors_ShouldThrowException()
        {
            // Arrange
            var isSuccess = true;
            var errors = new List<Error> { new Error("ErrorCode", "Error Description") };

            // Act
            Action action = () => new ConcreteObjectResult(isSuccess, errors);

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage("Invalid errors\nParameter name: errors");
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void Result_WhenCreateFailureResultWithNoErrors_ShouldThrowException()
        {
            // Arrange
            var isSuccess = false;
            IEnumerable<Error> nullErrors = null;
            var emptyErrors = Enumerable.Empty<Error>();

            // Act
            Action actionWithNullError = () => new ConcreteObjectResult(isSuccess, nullErrors);
            Action actionWithEmptyError = () => new ConcreteObjectResult(isSuccess, emptyErrors);

            // Assert
            actionWithNullError.Should().Throw<ArgumentException>().WithMessage("Invalid errors\nParameter name: errors");
            actionWithEmptyError.Should().Throw<ArgumentException>().WithMessage("Invalid errors\nParameter name: errors");
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void AddError_WithFailureResult_ShouldIncludeTheErrorInErrorsList()
        {
            // Arrange
            var errors = new List<Error> { 
                new Error("ErrorCode1", "Error Description1"), 
                new Error("ErrorCode2", "Error Description2") 
            };   
            var error = new Error("ErrorCode", "Error Description");
            var result = Result.Failure(errors);

            // Act
            result.AddError(error);

            // Assert
            result.Errors.Should().HaveCount(3);
            result.Errors.Should().Contain(error);
            result.Errors.Any(e => e.Code == "ErrorCode").Should().BeTrue();
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void FirstError_WithFailureResult_ShouldReturnFirstErrorInErrors()
        {
            // Arrange
            var errors = new List<Error> {
                new Error("ErrorCode1", "Error Description1"),
                new Error("ErrorCode2", "Error Description2")
            };
            var result = Result.Failure(errors);

            // Act
            var error = result.FirstError;

            // Assert
            error.Code.Should().Be("ErrorCode1");
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void FirstError_WithSuccessResult_ShouldReturnErrorNone()
        {
            // Arrange
            var result = Result.Success();

            // Act
            var error = result.FirstError;

            // Assert
            error.Should().Be(Error.None);
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void AddError_WithSuccessResult_ShouldThrowException()
        {
            // Arrange
            var error = new Error("ErrorCode", "Error Description");
            var result = Result.Success();

            // Act
            Action action = () => result.AddError(error);

            // Assert
            action.Should().Throw<InvalidOperationException>().WithMessage("Cannot add error to successful result");
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void ResultT_Success_ShouldReturnSuccessResultWithValue()
        {
            // Arrange
            var value = "Test With Value";

            // Act
            var result = Result<string>.Success(value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(value);
            result.Errors.Should().BeEmpty();
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void ResultT_Failure_ShouldReturnFailureResultWithErrors()
        {
            // Arrange
            var errors = new List<Error> { new Error("Test Error") };

            // Act
            var result = Result<string>.Failure(errors);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(errors);
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void ResultT_Failure_ShouldReturnFailureResultWithError()
        {
            // Arrange
            var error = new Error("Test Error");

            // Act
            var result = Result<string>.Failure(error);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.Should().Be(error);
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void ResultT_ImplicitConversion_ShouldReturnResult()
        {
            // Arrange
            var error = new Error("Test Error");
            var resultT = Result<string>.Failure(error);

            // Act
            Result result = resultT;

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.Should().Be(error);
        }

        [Trait("Unit.Domain", "Common")]
        [Fact]
        public void ResultT_WhenCreateSuccessResultWithNoValue_ShouldThrowException()
        {
            // Arrange
            var isSuccess = true;
            string value = null;
            IEnumerable<Error> errors = null;

            // Act
            Action action = () => new ConcreteObjectResultT<string>(isSuccess, value, errors);

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage("Invalid value\nParameter name: value");
        }
    }
}