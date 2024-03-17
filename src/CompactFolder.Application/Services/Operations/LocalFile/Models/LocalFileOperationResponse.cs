using CompactFolder.Domain.Operations.Contracts;
using System;
using System.Collections.Generic;

namespace CompactFolder.Application.Services.Operations.LocalFile.Models
{
    public class LocalFileOperationResponse
    {
        public Guid Id { get; set; }
        public string OriginPath { get; set; }
        public string OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public string DestinationPath { get; set; }
        public string DestinationFullPath { get; set; }
        public string CompressionPath { get; set; }
    }
}
