using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompactFolder.Domain.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> AsFailureResult<T>(this BaseResult result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Cannot convert a successful Result to Result<T> without a value.");
            }

            return Result<T>.Failure(result.Errors);
        }
    }
}
