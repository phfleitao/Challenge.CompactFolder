using CompactFolder.Domain.Contracts;
using CompactFolder.Domain.Operations;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace CompactFolder.Domain.Tests.Unit.TestUtils.ConcreteObjects
{
    public class ConcreteObjectOperation : Operation
    {
        public ConcreteObjectOperation(Guid id,
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            ITempPathProvider tempPathProvider = null)
            : base(id, originPath, outputFileName, exclusionRules, tempPathProvider) { }

        public ConcreteObjectOperation(TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            ITempPathProvider tempPathProvider = null)
            : base(originPath, outputFileName, exclusionRules, tempPathProvider) { }

        public new string GetTempPath()
        {
            return base.GetTempPath();
        }

        public void SetCompressionPath(TPath compressionPath)
        {
            CompressionPath = compressionPath;
        }
    }
}
