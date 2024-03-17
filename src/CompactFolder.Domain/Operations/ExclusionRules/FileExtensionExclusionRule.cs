using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CompactFolder.Domain.Operations.Contracts;

namespace CompactFolder.Domain.Operations.ExclusionRules
{
    public class FileExtensionExclusionRule : IExclusionRule
    {
        private readonly IEnumerable<string> _excludedFileExtensions;

        public FileExtensionExclusionRule(IEnumerable<string> excludedFileExtensions)
        {
            _excludedFileExtensions = excludedFileExtensions ?? Enumerable.Empty<string>();
        }

        public bool IsExcluded(string item)
        {
            var fileExtension = Path.GetExtension(item);
            return _excludedFileExtensions.Any(extension => 
                string.Equals(extension, fileExtension, StringComparison.OrdinalIgnoreCase));
        }
    }
}
