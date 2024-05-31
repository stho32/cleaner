using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.FileSystem;
using cleaner.Domain.Formatter;
using cleaner.Domain.Helpers;

namespace cleaner.Domain.FileBasedRules;

public class FileBasedQualityScanner
{
    private IFileSystemAccessProvider? _fileSystemAccessProvider;
    private int _totalFilesChecked;
    private int _totalFilesWithProblems;
    private int _totalProblems;
    private HashSet<string> _allowedUsings = null!;
    private static int _maxRuleIdWidth;

    public void PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker)
    {
        _maxRuleIdWidth = MaxRuleIdWidth();
        _allowedUsings = LoadAllowedUsingsOrUseDefault(commandLineOptions.AllowedUsingsFilePath);

        _fileSystemAccessProvider = new FileSystemAccessProvider();

        directoryWalker.Walk(
            ValidateRules,
            _fileSystemAccessProvider,
            "*.cs",
            commandLineOptions.DirectoryPath ?? "",
            commandLineOptions.StopOnFirstFileWithProblems);

        PrintStatistics(_totalFilesChecked, _totalFilesWithProblems, _totalProblems);
    }

    private static int MaxRuleIdWidth()
    {
        var rules = RuleFactory.GetRules(AllowedUsingsRule.GetDefaultAllowedUsings(), "");

        int maxRuleIdWidth = 0;

        foreach (var rule in rules)
        {
            int ruleIdWidth = rule.Id.Length;
            var currentRuleWidthIsLonger = ruleIdWidth > maxRuleIdWidth;

            if (currentRuleWidthIsLonger)
            {
                maxRuleIdWidth = ruleIdWidth;
            }
        }

        return maxRuleIdWidth;
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

    private static bool IsDesignerFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        return fileName.Contains(".Designer.");
    }

    private static void PrintStatistics(int totalFilesChecked, int totalFilesWithProblems, int totalProblems)
    {
        Console.WriteLine("\nStatistics:");
        Console.WriteLine($"  Total files checked:       {totalFilesChecked}");
        Console.WriteLine($"  Total files with problems: {totalFilesWithProblems}");
        Console.WriteLine($"  Total problems found:      {totalProblems}");
    }

    private bool ValidateRules(string filePath)
    {
        if (IsDesignerFile(filePath))
            return false;

        string? fileContent = GetFileContent(filePath);
        if (string.IsNullOrWhiteSpace(fileContent))
            return false;

        IncrementTotalFilesChecked();

        ValidationMessage[]? messages = RunValidationRules(filePath, fileContent);
        if (CollectionHelpers.IsNullOrEmpty(messages))
            return false;

        UpdateProblemStatistics(messages);
        PrintValidationMessages(messages);

        return true;
    }

    private string? GetFileContent(string filePath)
    {
        return _fileSystemAccessProvider?.GetFileContent(filePath);
    }

    private void IncrementTotalFilesChecked()
    {
        _totalFilesChecked += 1;
    }

    private ValidationMessage[] RunValidationRules(string filePath, string fileContent)
    {
        var rules = RuleFactory.GetRules(_allowedUsings, fileContent);
        var messages = new List<ValidationMessage>();

        foreach (var rule in rules)
        {
            messages.AddRange(rule.Validate(filePath, fileContent) ?? Array.Empty<ValidationMessage>());
        }

        return messages.ToArray();
    }

    private void UpdateProblemStatistics(ValidationMessage[]? messages)
    {
        if (messages == null)
            return;

        _totalFilesWithProblems += 1;
        _totalProblems += messages.Length;
    }

    private static void PrintValidationMessages(ValidationMessage[]? messages)
    {
        var messagePrinter = new ValidationMessagePrinter(_maxRuleIdWidth);
        messagePrinter.Print(messages);
    }
}