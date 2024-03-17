using CommandLine;
using CompactFolder.Domain.Enums;

namespace CompactFolder.Cli.Operations.Models
{
    public partial class Options : ILocalFileOutputTypeOptions
    {
        public string DestinationPath { get; set; }
    }

    internal interface ILocalFileOutputTypeOptions
    {
        [Option("destPath"
            ,SetName = nameof(OutputTypes.LocalFile) //Same SetName for Mutual Exclusive Parameters
            ,HelpText = @"Destination Path (e.g. C:\Folder)")]
        string DestinationPath { get; set; }
    }
}
