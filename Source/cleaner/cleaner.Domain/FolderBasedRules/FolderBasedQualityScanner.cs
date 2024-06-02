using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;

namespace cleaner.Domain.FolderBasedRules;

public class FolderBasedQualityScanner : IQualityScanner
{
    public ValidationMessage[] PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker)
    {
        throw new NotImplementedException();
    }
}