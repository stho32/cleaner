namespace cleaner.Domain.FileSystem;

public interface IFileSystemAccessProvider
{
    IEnumerable<string> GetDirectories(string path);
    IEnumerable<string> GetFiles(string path, string searchPattern);
    string GetFileName(string path);
}