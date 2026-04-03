using cleaner.Domain.FileSystem;

namespace cleaner.Domain.DirectoryTraversal;

public class LatestChangedFilesDirectoryWalker : IDirectoryWalker
{
    private int _lastNChangedFiles;
    private Func<string, bool>? _fileCallback;
    private IFileSystemAccessProvider? _fileSystemAccessProvider;
    private string? _searchPattern;

    public LatestChangedFilesDirectoryWalker(int lastNChangedFiles)
    {
        _lastNChangedFiles = lastNChangedFiles;
    }

    public void Walk(Func<string, bool> fileCallback, IFileSystemAccessProvider fileSystemAccessProvider,
        string searchPattern, string directoryPath, bool stopOnFirstFileWithErrors = false)
    {
        _fileCallback = fileCallback;
        _fileSystemAccessProvider = fileSystemAccessProvider;
        _searchPattern = searchPattern;

        if (IsNoValidPathGiven(directoryPath))
        {
            throw new ArgumentException("Invalid directory path provided.");
        }

        var allFiles = CollectAllFiles(directoryPath);
        var latestChangedFiles = allFiles
            .OrderByDescending(file => _fileSystemAccessProvider.GetLastWriteTimeUtc(file))
            .Take(_lastNChangedFiles)
            .ToList();

        foreach (var file in latestChangedFiles)
        {
            var hasFoundErrors = _fileCallback(file);
            var shouldStop = hasFoundErrors && stopOnFirstFileWithErrors;

            if (shouldStop)
                return;
        }
    }

    private bool IsNoValidPathGiven(string directoryPath)
    {
        return string.IsNullOrEmpty(directoryPath) || !_fileSystemAccessProvider!.DirectoryExists(directoryPath);
    }

    private List<string> CollectAllFiles(string directoryPath)
    {
        var files = new List<string>();

        void CollectFilesRecursively(string path)
        {
            var subdirectories = _fileSystemAccessProvider!.GetDirectories(path);

            foreach (var subdirectory in subdirectories)
            {
                if (DirectoryFilter.ShouldIgnore(subdirectory, _fileSystemAccessProvider!)) continue;

                CollectFilesRecursively(subdirectory);
            }

            var matchingFiles = _fileSystemAccessProvider.GetFiles(path, _searchPattern!);
            files.AddRange(matchingFiles);
        }

        CollectFilesRecursively(directoryPath);
        return files;
    }

}