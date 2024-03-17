using CommandLine;
using System.Collections.Generic;

namespace CompactFolder.Cli.Operations.Models
{
    public partial class Options
    {
        [Option('i', "inputPath", Required = true, HelpText = @"Input file path (e.g. C:\Temp)")]
        public string OriginPath { get; set; }

        [Option('o', "outputFile", Required = true, HelpText = "Output file name with extension (e.g. myfile.zip)")]
        public string DestinationFileName { get; set; }

        [Option("ex", Required = false, Separator = ',', HelpText = "File extension list to exclude (e.g. .bmp, .jpg, .txt)")]
        public IEnumerable<string> ExcludedFileExtensions { get; set; }

        [Option("ef", Required = false, Separator = ',', HelpText = "File name list to exclude (e.g. ficheiro1, filcheiro2)")]
        public IEnumerable<string> ExcludedFileNames { get; set; }

        [Option("ed", Required = false, Separator = ',', HelpText = "Directory list to exclude (e.g. git, diretório)")]
        public IEnumerable<string> ExcludedDirectories { get; set; }

        [Option('t', "outputType", Required = true, HelpText = "Output Type (e.g. localFile, filesShare, email)")]
        public string OutputType { get; set; }
    }
}
