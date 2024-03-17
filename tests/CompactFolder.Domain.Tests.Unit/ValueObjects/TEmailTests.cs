using CompactFolder.Domain.ValueObjects;
using FluentAssertions;
using System;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.ValueObjects
{
    public class TEmailTests
    {
        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData("test@example.com")]
        [InlineData("pedro.leitao@example.com.br")]
        [InlineData("user@sub.domain.example")]
        public void Create_ValidEmail_ShouldReturnSuccess(string validEmail)
        {
            // Act
            var email = TEmail.Create(validEmail);

            // Assert
            email.Should().NotBeNull();
            email.Address.Should().Be(validEmail.ToLowerInvariant());
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData("invalid-email")]
        [InlineData("missing@atSymbol")]
        [InlineData("double@@at@example.com")]
        [InlineData("")]
        public void Create_InvalidEmail_ShouldThrowsArgumentException(string invalidEmail)
        {
            // Act
            Action action = () => TEmail.Create(invalidEmail);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData("test@example.com", "test@example.com")]
        [InlineData("pedro@example.com", "pedro@example.com")]
        public void GetEqualityComponents_SameAddresses_ShouldReturnEqual(string firstEmail, string secondEmail)
        {
            // Act
            var firstEmailObj = TEmail.Create(firstEmail);
            var secondEmailObj = TEmail.Create(secondEmail);

            // Assert
            firstEmailObj.Should().Be(secondEmailObj);
            firstEmailObj.GetHashCode().Should().Be(secondEmailObj.GetHashCode());
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Theory]
        [InlineData("test@example.com", "pedro@example.com")]
        [InlineData("pedro@example.com", "test@example.com")]
        public void GetEqualityComponents_DifferentAddresses_ShouldReturnNotEqual(string firstEmail, string secondEmail)
        {
            // Act
            var firstEmailObj = TEmail.Create(firstEmail);
            var secondEmailObj = TEmail.Create(secondEmail);

            // Assert
            firstEmailObj.Should().NotBe(secondEmailObj);
            firstEmailObj.GetHashCode().Should().NotBe(secondEmailObj.GetHashCode());
        }

        [Trait("Unit.Domain", "ValueObjects")]
        [Fact]
        public void ToString_WithValidEmailAddress_ShouldReturnAddress()
        {
            // Arrange
            var email = TEmail.Create("email@email.com");

            // Act
            var emailString = email.ToString();

            // Assert
            emailString.Should().Be("email@email.com");
        }
    }
}
