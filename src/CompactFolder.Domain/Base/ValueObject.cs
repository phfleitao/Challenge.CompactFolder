using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactFolder.Domain.Base
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject a, ValueObject b) =>
            !(a == b);

        public virtual bool Equals(ValueObject other) =>
            other != null && ValuesAreEqual(other);

        public override bool Equals(object obj) =>
            obj is ValueObject valueObject && ValuesAreEqual(valueObject);

        public override int GetHashCode()
        {
            return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
        }

        private bool ValuesAreEqual(ValueObject valueObject) =>
            GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }
}
