using CompactFolder.Domain.Extensions;
using FluentValidation;

namespace CompactFolder.Domain.Operations.FileShare
{
    public class FileShareOperationValidator : OperationValidator<FileShareOperation>
    {
        public FileShareOperationValidator()
        {
            RuleFor(p => p.SharedPath)
                .NotEmpty().WithError(OperationErrors.Required)
                .Must(BeNetworkPath).WithError(OperationErrors.NotNetworkPath);
        }
    }
}
