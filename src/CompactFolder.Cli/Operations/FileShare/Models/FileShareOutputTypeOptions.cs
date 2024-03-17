using CommandLine;
using CompactFolder.Domain.Enums;

namespace CompactFolder.Cli.Operations.Models
{
    public partial class Options : IFileShareOutputTypeOptions
    {
        public string SharedPath { get; set; }
    }

    internal interface IFileShareOutputTypeOptions
    {
        [Option("sharedPath"
            ,SetName = nameof(OutputTypes.FileShare) //Same SetName for Mutual Exclusive Parameters
            , HelpText = @"Shared Path (e.g. \\Machine1\Folder)")]
        string SharedPath { get; set; }
    }
}
