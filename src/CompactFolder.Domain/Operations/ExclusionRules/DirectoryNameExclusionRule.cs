using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CompactFolder.Domain.Operations.Contracts;

namespace CompactFolder.Domain.Operations.ExclusionRules
{
    public class DirectoryNameExclusionRule : IExclusionRule
    {
        private readonly IEnumerable<string> _excludedDirectoryNames;
        public DirectoryNameExclusionRule(IEnumerable<string> excludedDirectoryNames)
        {
            _excludedDirectoryNames = excludedDirectoryNames ?? Enumerable.Empty<string>();
        }

        public bool IsExcluded(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
                return false;

            var lastDirectory = GetLastDirectoryName(item);

            if (string.IsNullOrWhiteSpace(lastDirectory))
                return false;

            return _excludedDirectoryNames.Any(directoryName => 
                string.Equals(directoryName, lastDirectory, StringComparison.OrdinalIgnoreCase));
        }

        //TODO: Create this in File Helper Class
        private string GetLastDirectoryName(string fullPath)
        {
            if (Path.HasExtension(fullPath))
                return new DirectoryInfo(fullPath)?.Parent?.Name;
            else
                return new DirectoryInfo(fullPath)?.Name
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
