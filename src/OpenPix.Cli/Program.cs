using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using OpenPix.Cli;

namespace OpenPix.Cli;

[ExcludeFromCodeCoverage]
class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = CliCommands.BuildRootCommand();
        return await rootCommand.InvokeAsync(args);
    }
}
