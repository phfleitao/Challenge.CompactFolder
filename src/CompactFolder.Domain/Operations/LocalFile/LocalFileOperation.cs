using CompactFolder.Domain.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;

namespace CompactFolder.Domain.Operations.LocalFile
{
    public sealed class LocalFileOperation : Operation
    {
        public TPath DestinationPath { get; private set; }
        public TPath DestinationFullPath => Path.Combine(DestinationPath.Path, OutputFileName.FileName);

        public LocalFileOperation(
            Guid id,
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            TPath destinationPath,
            ITempPathProvider tempPathProvider = null)
            : base(id, originPath, outputFileName, exclusionRules, tempPathProvider)
        {
            DestinationPath = destinationPath;
        }

        public LocalFileOperation(
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            TPath destinationPath, 
            ITempPathProvider tempPathProvider = null)
            : this(Guid.NewGuid(),
                  originPath,
                  outputFileName,
                  exclusionRules,
                  destinationPath,
                  tempPathProvider)
        { }
    }
}
