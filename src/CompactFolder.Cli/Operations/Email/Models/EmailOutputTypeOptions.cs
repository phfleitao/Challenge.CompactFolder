using CommandLine;
using CompactFolder.Domain.Enums;

namespace CompactFolder.Cli.Operations.Models
{
    public partial class Options : IEmailOutputTypeOptions
    {
        public string EmailTo { get; set; }
    }

    internal interface IEmailOutputTypeOptions
    {
        [Option("emailTo"
            ,SetName = nameof(OutputTypes.Email) //Same SetName for Mutual Exclusive Parameters
            ,HelpText = @"Destination Email (e.g. to@email.com)")]
        string EmailTo { get; set; }
    }
}
