using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.Formatter;

namespace cleaner.Domain;

public interface IQualityScanner
{
    ValidationMessage[] PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker);
}



