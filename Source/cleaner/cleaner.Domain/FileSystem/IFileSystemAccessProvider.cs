namespace cleaner.Domain.FileSystem;

public interface IFileSystemAccessProvider
{
    IEnumerable<string> GetDirectories(string path);
    IEnumerable<string> GetFiles(string path, string searchPattern);
    string GetFileName(string path);
    bool DirectoryExists(string path);
    string GetFileContent(string filePath);
    
    DateTime GetLastWriteTimeUtc(string filePath);
}