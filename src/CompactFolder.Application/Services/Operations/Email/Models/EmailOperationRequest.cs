using CompactFolder.Application.Contracts;
using CompactFolder.Domain.Operations.Contracts;
using System.Collections.Generic;

namespace CompactFolder.Application.Services.Operations.Email.Models
{
    public class EmailOperationRequest : IRequest
    {
        public string OriginPath { get; set; }
        public string OutputFileName { get; set; }
        public IEnumerable<IExclusionRule> ExclusionRules { get; set; }
        public string EmailTo { get; set; }
    }
}
