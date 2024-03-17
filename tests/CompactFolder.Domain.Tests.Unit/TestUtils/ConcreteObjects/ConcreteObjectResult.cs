using CompactFolder.Domain.Common;
using System.Collections.Generic;

namespace CompactFolder.Domain.Tests.Unit.TestUtils.ConcreteObjects
{
    public class ConcreteObjectResult : Result
    {
        public ConcreteObjectResult(bool isSuccess, IEnumerable<Error> errors) :
            base(isSuccess, errors)
        { }
    }
    public class ConcreteObjectResultT<T> : Result<T>
    {
        public ConcreteObjectResultT(bool isSuccess, T value, IEnumerable<Error> errors) : 
            base(isSuccess, value, errors)
        {  }
    }
}
