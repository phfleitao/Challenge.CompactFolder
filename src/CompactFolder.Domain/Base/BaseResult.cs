using CompactFolder.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactFolder.Domain.Base
{
    public abstract class BaseResult
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IEnumerable<Error> Errors { get; }
        public Error FirstError => IsFailure ? Errors.FirstOrDefault() : Error.None;

        protected BaseResult(bool isSuccess, IEnumerable<Error> errors)
        {
            if (IsSuccessWithErrors(isSuccess, errors) || IsFailureWithNoErrors(isSuccess, errors))
            {
                throw new ArgumentException("Invalid errors", nameof(errors));
            }

            IsSuccess = isSuccess;
            Errors = errors;
        }

        private bool IsSuccessWithErrors(bool isSuccess, IEnumerable<Error> errors)
        {
            return isSuccess && errors != null && errors.Any();
        }
        private bool IsFailureWithNoErrors(bool isSuccess, IEnumerable<Error> errors)
        {
            return !isSuccess && (errors is null || !errors.Any());
        }

        public void AddError(Error error)
        {
            if (IsSuccess)
            {
                throw new InvalidOperationException("Cannot add error to successful result");
            }

            ((List<Error>)Errors).Add(error);
        }
    }
}
