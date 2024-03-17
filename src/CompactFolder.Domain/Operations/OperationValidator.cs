using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using CompactFolder.Domain.Extensions;
using CompactFolder.Domain.ValueObjects;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;

namespace CompactFolder.Domain.Operations
{
    public class OperationValidator<T> : AbstractValidator<T> where T : Operation
    {
        public OperationValidator()
        {
            RuleFor(p => p.OriginPath)
                .NotEmpty().WithError(OperationErrors.Required)
                .Must(BeFullPathOrRootedDirectory).WithError(OperationErrors.NotFullPathOrDirectory);

            RuleFor(p => p.OutputFileName)
                .NotEmpty().WithError(OperationErrors.Required)
                .Must(BeOnlyFile).WithError(OperationErrors.NotOnlyFileName);

            RuleFor(p => p.CompressionPath)
                .NotEmpty().WithError(OperationErrors.Required)
                .Must(BeFullPath).WithError(OperationErrors.NotFullPath)
                .Must(BeZipFile).WithError(OperationErrors.NotZipFileExtension)
                .Must(NotBeNetworkPath).WithError(OperationErrors.IsNetworkPath);
        }

        protected bool BeFullPath(TPath path)
        {
            if (path == null)
                return false;

            return path.IsFullPath();
        }

        protected bool BeFullPathOrRootedDirectory(TPath path)
        {
            if (path == null)
                return false;

            return path.IsFullPath() || path.IsRootedDirectory();
        }

        protected bool BeZipFile(TPath path)
        {
            if (path == null)
                return false;

            return path.IsZipFile();
        }

        protected bool NotBeNetworkPath(TPath path)
        {
            if (path == null)
                return false;

            return !path.IsNetworkPath();
        }

        protected bool BeOnlyFile(TPath path)
        {
            if (path == null)
                return false;

            return path.IsOnlyFile();
        }
        protected bool BeRootedDirectory(TPath path)
        {
            if (path is null) return false;

            return path.IsRootedDirectory();
        }
        protected bool BeNetworkPath(TPath path)
        {
            if (path is null) return false;

            return path.IsNetworkPath();
        }

        public virtual async Task<BaseResult> ValidateAsync(T operation)
        {
            var validationResult = await base.ValidateAsync(operation);

            if (validationResult.IsValid)
            {
                return Result.Success();
            }
            else
            {
                var errors = validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage));
                return Result.Failure(errors);
            }
        }
    }
}
