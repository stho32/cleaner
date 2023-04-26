using cleaner.Domain.FileSystem;

namespace cleaner.Domain.DirectoryTraversal;

public class RecursiveDirectoryWalker : IDirectoryWalker
{
    private Func<string, bool>? _fileCallback;
    private IFileSystemAccessProvider? _fileSystemAccessProvider;
    private string? _searchPattern;

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

        WalkDirectory(directoryPath, stopOnFirstFileWithErrors);
    }

    private bool IsNoValidPathGiven(string directoryPath)
    {
        return string.IsNullOrEmpty(directoryPath) || !_fileSystemAccessProvider.DirectoryExists(directoryPath);
    }

    private void WalkDirectory(string directoryPath, bool stopOnFirstFileWithErrors)
    {
        var subdirectories = _fileSystemAccessProvider.GetDirectories(directoryPath);

        foreach (var subdirectory in subdirectories)
        {
            if (ShouldIgnoreThisFolder(subdirectory)) continue;

            WalkDirectory(subdirectory, stopOnFirstFileWithErrors);
        }

        var csFiles = _fileSystemAccessProvider.GetFiles(directoryPath, _searchPattern);

        ValidateFiles(stopOnFirstFileWithErrors, csFiles);
    }

    private void ValidateFiles(bool stopOnFirstFileWithErrors, IEnumerable<string> csFiles)
    {
        foreach (var csFile in csFiles)
        {
            var hasFoundErrors = _fileCallback(csFile);
            var shouldStop = hasFoundErrors && stopOnFirstFileWithErrors;

            if (shouldStop)
                Environment.Exit(1);
        }
    }

    private bool ShouldIgnoreThisFolder(string subdirectory)
    {
        var isGeneratedFolderOnLinux = subdirectory.Contains("/bin/") || subdirectory.Contains("/obj/");
        if (isGeneratedFolderOnLinux)
            return true;

        var isGeneratedFolderOnWindows = subdirectory.Contains("\\bin\\") || subdirectory.Contains("\\obj\\");
        if (isGeneratedFolderOnWindows)
            return true;

        var lastFolderName = _fileSystemAccessProvider.GetFileName(subdirectory);
        var lastFolderNameStartsWithADot = lastFolderName.StartsWith(".");

        if (lastFolderNameStartsWithADot)
            return true;

        return false;
    }
}