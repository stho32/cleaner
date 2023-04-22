using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.FileSystem;
using cleaner.Domain.Formatter;
using cleaner.Domain.Helpers;
using cleaner.Domain.Rules;

namespace cleaner.Domain;

public class QualityScanner
{
    private IFileSystemAccessProvider? _fileSystemAccessProvider;
    private int _totalFilesChecked;
    private int _totalFilesWithProblems;
    private int _totalProblems;
    private HashSet<string> _allowedUsings = null!;

    public void PerformQualityScan(CommandLineOptions commandLineOptions)
    {
        _allowedUsings = LoadAllowedUsingsOrUseDefault(commandLineOptions.AllowedUsingsFilePath);

        _fileSystemAccessProvider = new FileSystemAccessProvider();

        var walker = new DirectoryWalker(ValidateRules, new FileSystemAccessProvider(), "*.cs");
        walker.Walk(commandLineOptions.DirectoryPath ?? "", commandLineOptions.StopOnFirstFileWithProblems);

        PrintStatistics(_totalFilesChecked, _totalFilesWithProblems, _totalProblems);
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

    private  string? GetFileContent(string filePath)
    {
        return _fileSystemAccessProvider?.GetFileContent(filePath);
    }

    private  void IncrementTotalFilesChecked()
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
        var messagePrinter = new ValidationMessagePrinter();
        messagePrinter.Print(messages);
    }
}