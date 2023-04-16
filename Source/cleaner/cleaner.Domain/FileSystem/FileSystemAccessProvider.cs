namespace cleaner.Domain.FileSystem;

public class FileSystemAccessProvider : IFileSystemAccessProvider
{
    public IEnumerable<string> GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }

    public IEnumerable<string> GetFiles(string path, string searchPattern)
    {
        return Directory.GetFiles(path, searchPattern);
    }

    public string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }
}