using CompactFolder.Domain.Base;
using CompactFolder.Domain.Common;
using CompactFolder.Domain.Operations.Contracts;
using System.Collections.Generic;

namespace CompactFolder.Application.Services.CompressorService.Contracts
{
    public interface ICompressorCreator
    {
        BaseResult Create(string originPath, string destinationPath, IEnumerable<IExclusionRule> exclusionRules);
    }
}
