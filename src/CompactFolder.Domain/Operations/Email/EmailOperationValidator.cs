using CompactFolder.Domain.Extensions;
using FluentValidation;

namespace CompactFolder.Domain.Operations.Email
{
    public class EmailOperationValidator : OperationValidator<EmailOperation>
    {
        public EmailOperationValidator()
        {
            RuleFor(p => p.To)
                .NotEmpty().WithError(OperationErrors.Required);
        }
    }
}
