using CompactFolder.Cli.Operations.Models;
using CompactFolder.Domain.Base;
using System.Threading.Tasks;

namespace CompactFolder.Cli.Operations.Contracts
{
    public interface IOutputTypeHandler
    { 
        Task<BaseResult> Handle(Options options);
    }
}