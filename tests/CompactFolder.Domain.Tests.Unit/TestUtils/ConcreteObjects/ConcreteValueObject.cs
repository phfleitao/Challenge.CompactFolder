using CompactFolder.Domain.Base;
using System.Collections.Generic;

namespace CompactFolder.Domain.Tests.Unit.TestUtils.ConcreteObjects
{
    public class ConcreteValueObject : ValueObject
    {
        public string Property1 { get; }
        public int Property2 { get; }

        public ConcreteValueObject(string property1, int property2)
        {
            Property1 = property1;
            Property2 = property2;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Property1;
            yield return Property2;
        }
    }
}
