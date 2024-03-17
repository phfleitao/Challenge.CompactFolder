using CompactFolder.Domain.Base;

namespace CompactFolder.Domain.Common
{
    public sealed class Error
    {
        public string Code { get; }
        public string Description { get; }

        public static readonly Error None = new Error(string.Empty);

        public Error(string code, string description = null)
        {
            Code = code;
            Description = description;
        }
    }
}
