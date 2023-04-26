namespace cleaner.Domain.DirectoryTraversal;

public interface IDirectoryWalker
{
    void Walk(string directoryPath, bool stopOnFirstFileWithErrors = false);
}