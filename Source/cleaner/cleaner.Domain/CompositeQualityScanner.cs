using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;

namespace cleaner.Domain;

public class CompositeQualityScanner : IQualityScanner
{
    private readonly IQualityScanner[] _scanners;

    public CompositeQualityScanner(params IQualityScanner[] scanners)
    {
        _scanners = scanners;
    }

    public ValidationMessage[] PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker)
    {
        var allMessages = new List<ValidationMessage>();

        foreach (var scanner in _scanners)
        {
            var messages = scanner.PerformQualityScan(commandLineOptions, directoryWalker);
            allMessages.AddRange(messages);
        }

        return allMessages.ToArray();
    }
}