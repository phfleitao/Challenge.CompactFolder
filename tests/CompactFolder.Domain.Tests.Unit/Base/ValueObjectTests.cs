using CompactFolder.Domain.Base;
using CompactFolder.Domain.Tests.Unit.TestUtils.ConcreteObjects;
using FluentAssertions;
using Xunit;

namespace CompactFolder.Domain.Tests.Unit.Base
{
    public class ValueObjectTests
    {
        [Trait("Unit.Domain", "Base")]
        [Fact]
        public void ValueObject_EqualObjects_ShouldBeEqual()
        {
            // Arrange
            ValueObject valueObject1 = new ConcreteValueObject("abc", 123);
            ValueObject valueObject2 = new ConcreteValueObject("abc", 123);
            ValueObject valueObjectWithNull1 = new ConcreteValueObject(null, 123);
            ValueObject valueObjectWithNull2 = new ConcreteValueObject(null, 123);
            object objectValueObject = new ConcreteValueObject("abc", 123);

            // Act/Assert
            valueObject1.Should().Be(valueObject2);
            (valueObject1 == valueObject2).Should().BeTrue();
            (valueObject1 != valueObject2).Should().BeFalse();
            valueObject1.Equals(valueObject2).Should().BeTrue();
            valueObject1.Equals(objectValueObject).Should().BeTrue();
            valueObject1.GetHashCode().Should().Be(valueObject2.GetHashCode());
            valueObjectWithNull1.GetHashCode().Should().Be(valueObjectWithNull2.GetHashCode());
        }

        [Trait("Unit.Domain", "Base")]
        [Fact]
        public void ValueObject_NotEqualObjects_ShouldNotBeEqual()
        {
            // Arrange
            ValueObject valueObject1 = new ConcreteValueObject("abc", 123);
            ValueObject valueObject2 = new ConcreteValueObject("xyz", 456);
            ValueObject valueObjectWithNull1 = new ConcreteValueObject(null, 123);
            ValueObject valueObjectWithNull2 = new ConcreteValueObject(null, 456);
            object objectValueObject = new ConcreteValueObject("xyz", 789);

            // Act/Assert
            valueObject1.Should().NotBe(valueObject2);
            (valueObject1 == valueObject2).Should().BeFalse();
            (valueObject1 != valueObject2).Should().BeTrue();
            valueObject1.Equals(valueObject2).Should().BeFalse();
            valueObject1.Equals(objectValueObject).Should().BeFalse();
            valueObject1.GetHashCode().Should().NotBe(valueObject2.GetHashCode());
            valueObjectWithNull1.GetHashCode().Should().NotBe(valueObjectWithNull2.GetHashCode());
        }

        [Trait("Unit.Domain", "Base")]
        [Fact]
        public void ValueObject_NullComparison_ShouldNotBeEqual()
        {
            // Arrange
            ValueObject valueObject = new ConcreteValueObject("abc", 123);
            ValueObject valueObjectNull = null;

            // Act/Assert
            (valueObject == valueObjectNull).Should().BeFalse();
            (valueObjectNull == valueObject).Should().BeFalse();
            (valueObject != valueObjectNull).Should().BeTrue();
            (valueObjectNull != valueObject).Should().BeTrue();
        }

        [Trait("Unit.Domain", "Base")]
        [Fact]
        public void ValueObject_BothNullComparison_ShouldBeEqual()
        {
            // Arrange
            ConcreteValueObject valueObject1 = null;
            ConcreteValueObject valueObject2 = null;

            // Act/Assert
            (valueObject1 == valueObject2).Should().BeTrue();
            (valueObject1 != valueObject2).Should().BeFalse();
        }
    }
}
