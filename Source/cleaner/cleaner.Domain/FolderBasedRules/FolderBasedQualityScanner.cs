using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.FileSystem;

namespace cleaner.Domain.FolderBasedRules;

public class FolderBasedQualityScanner : IQualityScanner
{
    private readonly int _maximumCodeFilesPerDirectory;
    private readonly IStatisticsCollector _statisticsCollector;
    private readonly List<ValidationMessage> _validationMessages = new();
    private Dictionary<string,int> _fileCountByDirectory = new();

    public FolderBasedQualityScanner(int maximumCodeFilesPerDirectory, IStatisticsCollector statisticsCollector)
    {
        _maximumCodeFilesPerDirectory = maximumCodeFilesPerDirectory;
        _statisticsCollector = statisticsCollector;
    }

    public ValidationMessage[] PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker)
    {
        var fileSystemAccessProvider = new FileSystemAccessProvider();

        directoryWalker.Walk(
            CollectDirectories,
            fileSystemAccessProvider,
            "*.cs",
            commandLineOptions.DirectoryPath ?? "",
            commandLineOptions.StopOnFirstFileWithProblems);

        ConvertDirectoriesWithMoreThanNFilesToValidationMessages();


        return _validationMessages.ToArray();
    }

    private void ConvertDirectoriesWithMoreThanNFilesToValidationMessages()
    {
        foreach (var directory in _fileCountByDirectory)
        {
            _statisticsCollector.ScanningFolder();

            if (GreaterThanMaximumFileCount(directory.Value))
            {
                _statisticsCollector.FoundFolderWithProblems();

                _validationMessages.Add(new ValidationMessage("FOLDER1", "TOO_MANY_FILES", directory.Key + " contains " +
                    directory.Value + " files. (That is more than the allowed amount of " +
                    _maximumCodeFilesPerDirectory + "). Please rethink your structure here, you may need more subfolders."));
            }
        }
    }

    private bool CollectDirectories(string filePath)
    {
        var fileFilter = new CSharpCodeFileFilter();
        if (!fileFilter.IsValidFilename(filePath))
            return false;

        string? directoryPath = Path.GetDirectoryName(filePath);

        _fileCountByDirectory.TryGetValue(directoryPath!, out var count);
        _fileCountByDirectory[directoryPath!] = count + 1;

        return GreaterThanMaximumFileCount(_fileCountByDirectory[directoryPath!]);
    }

    private bool GreaterThanMaximumFileCount(int fileCount)
    {
        return fileCount > _maximumCodeFilesPerDirectory;
    }
}