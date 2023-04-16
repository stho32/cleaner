using cleaner.Domain.FileSystem;

namespace cleaner.Domain;

public class DirectoryWalker
{
    private readonly Action<string> _fileCallback;
    private readonly IFileSystemAccessProvider _fileSystemAccessProvider;

    public DirectoryWalker(Action<string> fileCallback, IFileSystemAccessProvider fileSystemAccessProvider)
    {
        _fileCallback = fileCallback;
        _fileSystemAccessProvider = fileSystemAccessProvider;
    }

    public void Walk(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath) || !_fileSystemAccessProvider.GetDirectories(directoryPath).GetEnumerator().MoveNext())
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
            if (!_fileSystemAccessProvider.GetFileName(subdirectory).StartsWith("."))
            {
                WalkDirectory(subdirectory);
            }
        }

        var csFiles = _fileSystemAccessProvider.GetFiles(directoryPath, "*.cs");

        foreach (var csFile in csFiles)
        {
            _fileCallback(csFile);
        }
    }
}