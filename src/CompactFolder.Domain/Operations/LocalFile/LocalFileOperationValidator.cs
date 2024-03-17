using CompactFolder.Domain.Extensions;
using FluentValidation;

namespace CompactFolder.Domain.Operations.LocalFile
{
    public class LocalFileOperationValidator : OperationValidator<LocalFileOperation>
    {
        public LocalFileOperationValidator()
        {
            RuleFor(p => p.DestinationPath)
                .NotEmpty().WithError(OperationErrors.Required)
                .Must(BeRootedDirectory).WithError(OperationErrors.NotRootedDirectoryPath)
                .Must(NotBeNetworkPath).WithError(OperationErrors.IsNetworkPath);
        }
    }
}
