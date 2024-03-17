using CompactFolder.Domain.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using CompactFolder.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompactFolder.Domain.Operations
{
    public abstract class Operation
    {
        private readonly ITempPathProvider _tempPathProvider;
        public Guid Id { get; protected set; }
        public TPath OriginPath { get; protected set; }
        public TPath OutputFileName { get; protected set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; protected set; }
        public TPath CompressionPath { get; protected set; }

        protected Operation(
            Guid id,
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            ITempPathProvider tempPathProvider = null)
        {
            _tempPathProvider = tempPathProvider;

            Id = id;
            OriginPath = originPath;
            OutputFileName = outputFileName;
            ExclusionRules = exclusionRules is null ? Enumerable.Empty<IExclusionRule>() : exclusionRules;
            
            InitializeCompressionPath();
        }

        private void InitializeCompressionPath()
        {
            if(OutputFileName != null)
                CompressionPath = Path.Combine(GetTempPath(), OutputFileName.FileName);
        }

        protected Operation(
            TPath originPath,
            TPath outputFileName,
            IEnumerable<IExclusionRule> exclusionRules,
            ITempPathProvider tempPathProvider = null) :
            this(Guid.NewGuid(),
                originPath,
                outputFileName,
                exclusionRules,
                tempPathProvider)
        { }

        protected virtual string GetTempPath()
        {
            if(_tempPathProvider?.GetTempPath() != null)
                return _tempPathProvider.GetTempPath();
            
            return Path.GetTempPath();
        }
    }
}
