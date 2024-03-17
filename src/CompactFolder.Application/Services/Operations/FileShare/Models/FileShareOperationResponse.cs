using CompactFolder.Domain.Operations.Contracts;
using System;
using System.Collections.Generic;

namespace CompactFolder.Application.Services.Operations.FileShare.Models
{
    public class FileShareOperationResponse
    {
        public Guid Id { get; set; }
        public string OriginPath { get; set; }
        public string OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public string SharedPath { get; set; }
        public string SharedFullPath { get; set; }
        public string CompressionPath { get; set; }
    }
}
