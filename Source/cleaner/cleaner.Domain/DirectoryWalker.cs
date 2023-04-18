using cleaner.Domain.FileSystem;

namespace cleaner.Domain;

public class DirectoryWalker
{
    private readonly Action<string> _fileCallback;
    private readonly IFileSystemAccessProvider _fileSystemAccessProvider;
    private readonly string _searchPattern;

    public DirectoryWalker(Action<string> fileCallback, IFileSystemAccessProvider fileSystemAccessProvider,
        string searchPattern)
    {
        _fileCallback = fileCallback;
        _fileSystemAccessProvider = fileSystemAccessProvider;
        _searchPattern = searchPattern;
    }

    public void Walk(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath) || !_fileSystemAccessProvider.DirectoryExists(directoryPath))
        {
            throw new ArgumentException("Invalid directory path provided.");
        }

        WalkDirectory(directoryPath);
    }

    private void WalkDirectory(string directoryPath)
    {
        var subdirectories = _fileSystemAccessProvider.GetDirectories(directoryPath);

        foreach (var subdirectory in subdirectories)
        {
            if (subdirectory.Contains("/bin/") || subdirectory.Contains("/obj/"))
                continue;
            if (_fileSystemAccessProvider.GetFileName(subdirectory).StartsWith("."))
                continue;

            WalkDirectory(subdirectory);
        }

        var csFiles = _fileSystemAccessProvider.GetFiles(directoryPath, _searchPattern);

        foreach (var csFile in csFiles)
        {
            _fileCallback(csFile);
        }
    }
}