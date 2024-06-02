using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.FileSystem;
using cleaner.Domain.Helpers;

namespace cleaner.Domain.FileBasedRules;

public class FileBasedQualityScanner : IQualityScanner
{
    private IFileSystemAccessProvider? _fileSystemAccessProvider;
    private HashSet<string> _allowedUsings = null!;
    private List<ValidationMessage> _validationMessages = new();

    public ValidationMessage[] PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker)
    {
        _allowedUsings = LoadAllowedUsingsOrUseDefault(commandLineOptions.AllowedUsingsFilePath);

        _fileSystemAccessProvider = new FileSystemAccessProvider(); 

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

    private static bool IsDesignerFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        return fileName.Contains(".Designer.");
    }

    private bool ValidateRules(string filePath)
    {
        if (IsDesignerFile(filePath))
            return false;

        string? fileContent = GetFileContent(filePath);
        if (string.IsNullOrWhiteSpace(fileContent))
            return false;

        ValidationMessage[]? messages = RunValidationRules(filePath, fileContent);
        _validationMessages.AddRange(messages);

        if (CollectionHelpers.IsNullOrEmpty(messages))
            return false;

        return true;
    }

    private string? GetFileContent(string filePath)
    {
        return _fileSystemAccessProvider?.GetFileContent(filePath);
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
}

