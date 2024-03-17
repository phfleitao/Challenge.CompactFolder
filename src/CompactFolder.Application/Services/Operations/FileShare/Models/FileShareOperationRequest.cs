using CompactFolder.Application.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using System.Collections.Generic;

namespace CompactFolder.Application.Services.Operations.FileShare.Models
{
    public class FileShareOperationRequest : IRequest
    {
        public string OriginPath { get; set; }
        public string OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public string SharedPath { get; set; }
    }
}
