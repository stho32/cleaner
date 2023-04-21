using cleaner.Domain.FileSystem;

namespace cleaner.Domain;

public class DirectoryWalker
{
    private readonly Func<string, bool> _fileCallback;
    private readonly IFileSystemAccessProvider _fileSystemAccessProvider;
    private readonly string _searchPattern;

    public DirectoryWalker(Func<string, bool> fileCallback, IFileSystemAccessProvider fileSystemAccessProvider,
        string searchPattern)
    {
        _fileCallback = fileCallback;
        _fileSystemAccessProvider = fileSystemAccessProvider;
        _searchPattern = searchPattern;
    }

    public void Walk(string directoryPath, bool stopOnFirstFileWithErrors = false)
    {
        if (string.IsNullOrEmpty(directoryPath) || !_fileSystemAccessProvider.DirectoryExists(directoryPath))
        {
            throw new ArgumentException("Invalid directory path provided.");
        }

        WalkDirectory(directoryPath, stopOnFirstFileWithErrors);
    }

    private void WalkDirectory(string directoryPath, bool stopOnFirstFileWithErrors)
    {
        var subdirectories = _fileSystemAccessProvider.GetDirectories(directoryPath);

        foreach (var subdirectory in subdirectories)
        {
            if (subdirectory.Contains("/bin/") || subdirectory.Contains("/obj/"))
                continue;
            if (subdirectory.Contains("\\bin\\") || subdirectory.Contains("\\obj\\"))
                continue;
            if (_fileSystemAccessProvider.GetFileName(subdirectory).StartsWith("."))
                continue;

            WalkDirectory(subdirectory, stopOnFirstFileWithErrors);
        }

        var csFiles = _fileSystemAccessProvider.GetFiles(directoryPath, _searchPattern);

        foreach (var csFile in csFiles)
        {
            if (_fileCallback(csFile) && stopOnFirstFileWithErrors)
                Environment.Exit(1);
        }
    }
}