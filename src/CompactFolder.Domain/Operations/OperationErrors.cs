using CompactFolder.Domain.Common;

namespace CompactFolder.Domain.Operations
{
    public class OperationErrors
    {
        public static readonly Error Required = new Error("Operation.Required", "Required property {PropertyName}");
        public static readonly Error NotDirectoryPath = new Error("Operation.NotDirectoryPath", "The provided path in the {PropertyName} is not a directory");
        public static readonly Error NotRootedDirectoryPath = new Error("Operation.NotRootedDirectoryPath", "The provided path in the {PropertyName} is not a rooted directory");
        public static readonly Error IsNetworkPath = new Error("Operation.IsNetworkPath", "The provided path in the {PropertyName} is a network path");
        public static readonly Error NotNetworkPath = new Error("Operation.NotNetworkPath", "The provided path in the {PropertyName} is not a network path");
        public static readonly Error NotFullPath = new Error("Operation.NotFullPath", "The provided path in the {PropertyName} is not a full path");
        public static readonly Error NotOnlyFileName = new Error("Operation.NotOnlyFileName", "{PropertyName} is not only a file name with extension");
        public static readonly Error NotZipFileExtension = new Error("Operation.NotZipFileExtension", "The provided path in the {PropertyName} is not a zip file");
        public static readonly Error NotFullPathOrDirectory = new Error("Operation.NotFullPathOrDirectory", "The provided path in the {PropertyName} is not a full path or a directory");
    }
}
