using CompactFolder.Domain.Base;
using System;
using System.Threading.Tasks;

namespace CompactFolder.Cli
{
    public interface IStartupApplication : IDisposable
    {
        Task<BaseResult> RunAsync(string[] args);
    }
}
