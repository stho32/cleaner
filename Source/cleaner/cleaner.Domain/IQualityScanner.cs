using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;

namespace cleaner.Domain;

public interface IQualityScanner
{
    ValidationMessage[] PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker);
}