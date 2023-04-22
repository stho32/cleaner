using CommandLine;
using CommandLine.Text;

namespace cleaner.Domain.CommandLineArguments;

public class CommandLineOptions
{
    [Option('d', "directory", Required = false, HelpText = "The path of the directory to be processed.")]
    public string DirectoryPath { get; set; }

    [Option('s', "stopOnFirstFileWithProblems", Required = false, HelpText = "If present, the process will stop on the first file with problems.")]
    public bool StopOnFirstFileWithProblems { get; set; }

    [Option("allowedUsings", Required = false, HelpText = "A file containing allowed using statements, one per line.")]
    public string AllowedUsingsFilePath { get; set; }

    [Option('r', "listRules", Required = false, HelpText = "List all existing rules.")]
    public bool ListRules { get; set; }

    [Usage(ApplicationAlias = "cleaner")]
    public static IEnumerable<Example> Examples
    {
        get
        {
            return new List<Example>()
            {
                new Example("Processing a directory without stopping on first file with problems and without allowed usings file",
                    new CommandLineOptions { DirectoryPath = @"C:\projects" }),
                new Example("Processing a directory and stopping on first file with problems",
                    new CommandLineOptions { DirectoryPath = @"C:\projects", StopOnFirstFileWithProblems = true }),
                new Example("Processing a directory with an allowed usings file",
                    new CommandLineOptions { DirectoryPath = @"C:\projects", AllowedUsingsFilePath = @"C:\allowedUsings.txt" }),
                new Example("Listing all existing rules",
                    new CommandLineOptions { ListRules = true })
            };
        }
    }
}