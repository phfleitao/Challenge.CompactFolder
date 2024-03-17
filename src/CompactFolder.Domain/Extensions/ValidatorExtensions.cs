using CompactFolder.Domain.Common;
using FluentValidation;

namespace CompactFolder.Domain.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            Error error)
        {
            rule.WithErrorCode(error.Code)
                .WithMessage(error.Description);

            return rule;
        }
    }
}
