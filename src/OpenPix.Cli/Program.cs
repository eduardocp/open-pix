using System.CommandLine;
using System.CommandLine.Parsing;
using OpenPix.Cli;

var rootCommand = CliCommands.BuildRootCommand();
return await rootCommand.InvokeAsync(args);
