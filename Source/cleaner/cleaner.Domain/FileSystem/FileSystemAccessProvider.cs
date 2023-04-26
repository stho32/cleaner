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
    
    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public string GetFileContent(string filePath)
    {
        return File.ReadAllText(filePath);
    }
    
    public DateTime GetLastWriteTimeUtc(string filePath)
    {
        return File.GetLastWriteTimeUtc(filePath);
    }
}