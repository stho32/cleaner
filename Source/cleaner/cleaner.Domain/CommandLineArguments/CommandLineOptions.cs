using CommandLine;
using CommandLine.Text;

namespace cleaner.Domain.CommandLineArguments;

public class CommandLineOptions
{
    [Option('d', "directory", Required = false, HelpText = "The path of the directory to be processed.")]
    public string DirectoryPath { get; private set; } = null!;

    [Option('s', "stopOnFirstFileWithProblems", Required = false, HelpText = "If present, the process will stop on the first file with problems.")]
    public bool StopOnFirstFileWithProblems { get; private set; }

    [Option("allowedUsings", Required = false, HelpText = "A file containing allowed using statements, one per line.")]
    public string AllowedUsingsFilePath { get; private set; } = null!;

    [Option('r', "listRules", Required = false, HelpText = "List all existing rules.")]
    public bool ListRules { get; private set; }

    [Option("latestChangedFiles", Required = false, HelpText = "If present, the process will scan only the latest changed n files.")]
    public int? LatestChangedFiles { get; private set; }

    [Usage(ApplicationAlias = "cleaner")]
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<Example> Examples =>
        new List<Example>
        {
            new("Processing a directory without stopping on first file with problems and without allowed usings file",
                new CommandLineOptions { DirectoryPath = @"C:\projects" }),
            new("Processing a directory and stopping on first file with problems",
                new CommandLineOptions { DirectoryPath = @"C:\projects", StopOnFirstFileWithProblems = true }),
            new("Processing a directory with an allowed usings file",
                new CommandLineOptions { DirectoryPath = @"C:\projects", AllowedUsingsFilePath = @"C:\allowedUsings.txt" }),
            new("Listing all existing rules",
                new CommandLineOptions { ListRules = true }),
            new("Processing a directory with the latest changed 5 files",
                new CommandLineOptions { DirectoryPath = @"C:\projects", LatestChangedFiles = 5 })
        };
}
