using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.Configuration;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.FileSystem;
using cleaner.Domain.Helpers;
using Microsoft.CodeAnalysis.CSharp;

namespace cleaner.Domain.FileBasedRules;

public class FileBasedQualityScanner : IQualityScanner
{
    private readonly IStatisticsCollector _statisticsCollector;
    private readonly IFileSystemAccessProvider _fileSystemAccessProvider;
    private HashSet<string> _allowedUsings = null!;
    private CleanerConfig _config = new();
    private List<ValidationMessage> _validationMessages = new();

    public FileBasedQualityScanner(IStatisticsCollector statisticsCollector)
        : this(statisticsCollector, new FileSystemAccessProvider())
    {
    }

    public FileBasedQualityScanner(IStatisticsCollector statisticsCollector, IFileSystemAccessProvider fileSystemAccessProvider)
    {
        _statisticsCollector = statisticsCollector;
        _fileSystemAccessProvider = fileSystemAccessProvider;
    }

    public ValidationMessage[] PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker)
    {
        _allowedUsings = LoadAllowedUsingsOrUseDefault(commandLineOptions.AllowedUsingsFilePath);
        _config = CleanerConfigLoader.Load(commandLineOptions.DirectoryPath ?? ".");

        directoryWalker.Walk(
            ValidateRules,
            _fileSystemAccessProvider,
            "*.cs",
            commandLineOptions.DirectoryPath ?? "",
            commandLineOptions.StopOnFirstFileWithProblems);

        return _validationMessages.ToArray();
    }

    private static HashSet<string> LoadAllowedUsingsOrUseDefault(string? allowedUsingsFilePath)
    {
        if (!string.IsNullOrEmpty(allowedUsingsFilePath))
        {
            try
            {
                return new HashSet<string>(File.ReadAllLines(allowedUsingsFilePath));
            }
            catch (IOException e)
            {
                Console.WriteLine($"Error reading allowed usings file: {e.Message}");
            }
        }

        return AllowedUsingsRule.GetDefaultAllowedUsings();
    }

    private bool ValidateRules(string filePath)
    {
        var fileFilter = new CSharpCodeFileFilter();
        if (!fileFilter.IsValidFilename(filePath))
            return false;

        string? fileContent = _fileSystemAccessProvider.GetFileContent(filePath);
        if (!fileFilter.IsValidContent(fileContent))
            return false;

        _statisticsCollector.ScanningFile();

        ValidationMessage[] messages = RunValidationRules(filePath, fileContent!);
        _validationMessages.AddRange(messages);

        if (CollectionHelpers.IsNullOrEmpty(messages))
            return false;

        _statisticsCollector.FoundFileWithProblems();

        return true;
    }

    private ValidationMessage[] RunValidationRules(string filePath, string fileContent)
    {
        var rules = RuleFactory.GetRules(_allowedUsings, fileContent, _config);
        var tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        var messages = new List<ValidationMessage>();

        foreach (var rule in rules)
        {
            messages.AddRange(rule.Validate(filePath, fileContent, tree, root));
        }

        return messages.ToArray();
    }
}
