using CompactFolder.Domain.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;

namespace CompactFolder.Domain.Operations.FileShare
{
    public sealed class FileShareOperation : Operation
    {
        public TPath SharedPath { get; private set; }
        public TPath SharedFullPath => Path.Combine(SharedPath.Path, OutputFileName.FileName);

        public FileShareOperation(
            Guid id,
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            TPath sharedPath, 
            ITempPathProvider tempPathProvider = null)
            : base(id, originPath, outputFileName, exclusionRules, tempPathProvider)
        {
            SharedPath = sharedPath;
        }

        public FileShareOperation(
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            TPath sharedPath, 
            ITempPathProvider tempPathProvider = null)
            : this(Guid.NewGuid(),
                  originPath,
                  outputFileName,
                  exclusionRules,
                  sharedPath,
                  tempPathProvider)
        { }
    }
}
