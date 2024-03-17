using CompactFolder.Cli.Operations.Models;
using System;

namespace CompactFolder.Cli.Exceptions
{
    public class InvalidArgsOptionsException : Exception
    {
        public string OptionName { get; }

        public InvalidArgsOptionsException(string optionName) 
            : base($"Invalid argument {optionName}")
        {
            OptionName = optionName;           
        }
    }
}
