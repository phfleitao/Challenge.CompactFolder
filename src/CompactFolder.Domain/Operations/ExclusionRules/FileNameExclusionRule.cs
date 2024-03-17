using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CompactFolder.Domain.Operations.Contracts;

namespace CompactFolder.Domain.Operations.ExclusionRules
{
    public class FileNameExclusionRule : IExclusionRule
    {
        private readonly IEnumerable<string> _excludedFileNames;

        public FileNameExclusionRule(IEnumerable<string> excludedFileNames)
        {
            _excludedFileNames = excludedFileNames ?? Enumerable.Empty<string>();
        }

        public bool IsExcluded(string item)
        {
            var file = Path.GetFileNameWithoutExtension(item);
            return _excludedFileNames.Any(fileName => 
                string.Equals(fileName, file, StringComparison.OrdinalIgnoreCase));
        }
    }
}
