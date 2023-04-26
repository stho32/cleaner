using cleaner.Domain.FileSystem;

namespace cleaner.Domain.DirectoryTraversal;

public interface IDirectoryWalker
{
    void Walk(Func<string, bool> fileCallback, IFileSystemAccessProvider fileSystemAccessProvider,
        string searchPattern, string directoryPath, bool stopOnFirstFileWithErrors = false);
}