using CompactFolder.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactFolder.Domain.Common
{
    public class Result : BaseResult
    {
        public Result(bool isSuccess, IEnumerable<Error> errors) : base(isSuccess, errors) { }

        public static Result Success() => new Result(true, Enumerable.Empty<Error>());
        public static Result Failure(IEnumerable<Error> errors) => new Result(false, errors);
        public static Result Failure(Error error) => new Result(false, new List<Error> { error });
    }

    public class Result<T> : BaseResult
    {
        public T Value { get; }

        protected Result(bool isSuccess, T value, IEnumerable<Error> errors) : base(isSuccess, errors)
        {
            if (isSuccess && value == null)
            {
                throw new ArgumentException("Invalid value", nameof(value));
            }

            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, Enumerable.Empty<Error>());
        public static Result<T> Failure(IEnumerable<Error> errors) => new Result<T>(false, default(T), errors);
        public static Result<T> Failure(Error error) => new Result<T>(false, default(T), new List<Error> { error });

        public static implicit operator Result (Result<T> result) => new Result(result.IsSuccess, result.Errors);
    }
}
