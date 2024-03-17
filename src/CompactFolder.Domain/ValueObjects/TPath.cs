using CompactFolder.Domain.Base;
using CompactFolder.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IO = System.IO;

namespace CompactFolder.Domain.ValueObjects
{
    public class TPath : ValueObject
    {
        public string Path { get; }
        public string FileName => IO.Path.GetFileName(Path);
        public string FileExtension => IO.Path.GetExtension(Path);

        private TPath(string path)
        {
            Path = path;
            
            Validate();
        }

        private TPath(string path, string filename) : this(IO.Path.Combine(path, filename)) { }
        
        public static TPath Create(string path)
        {
            return new TPath(path);
        }

        public static TPath Create(string path, string filename)
        {
            return new TPath(path, filename);
        }

        public static implicit operator TPath(string path)
        {
            return TPath.Create(path);
        }

        public bool IsFile()
        {
            return IO.Path.HasExtension(Path);
        }
        public bool IsOnlyFile()
        {
            return IsFile() && Path.Equals(FileName, StringComparison.OrdinalIgnoreCase);
        }
        public bool IsFullPath()
        {
            return IO.Path.IsPathRooted(Path) && IsFile();
        }
        public bool IsDirectory()
        {
            return !IO.Path.HasExtension(Path);
        }

        public bool IsRootedDirectory()
        {
            return IO.Path.IsPathRooted(Path) && IsDirectory();
        }
        public bool IsRelativePath()
        {
            return !IO.Path.IsPathRooted(Path);
        }

        public bool IsNetworkPath()
        {
            return Path.StartsWith(@"\\");
        }
        public bool IsZipFile()
        {
            return IO.Path.HasExtension(Path) && FileExtension.Equals(".zip", StringComparison.OrdinalIgnoreCase);
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Path) ||
                ContainsInvalidPathCharacters())
                throw new ArgumentException("Invalid path: ", nameof(Path));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Path.ToLowerInvariant();
        }

        private bool ContainsInvalidPathCharacters()
        {
            char[] invalidPathChars = IO.Path.GetInvalidPathChars();
            return Path.IndexOfAny(invalidPathChars) != -1;
        }

        public override string ToString()
        {
            return this.Path;
        }
    }
}
