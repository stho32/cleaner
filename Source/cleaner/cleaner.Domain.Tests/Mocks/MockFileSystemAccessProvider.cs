using System.Text.RegularExpressions;
using cleaner.Domain.FileSystem;

namespace cleaner.Domain.Tests.Mocks;

public class MockFileSystemAccessProvider : IFileSystemAccessProvider
{
    public Dictionary<string, List<string>> Directories { get; } = new();
    public Dictionary<string, List<string>> Files { get; } = new();

    public IEnumerable<string> GetDirectories(string path)
    {
        return Directories.TryGetValue(path, out var dirs) ? dirs : Array.Empty<string>();
    }

    public IEnumerable<string> GetFiles(string path, string searchPattern)
    {
        if (!Files.TryGetValue(path, out var files))
            return Array.Empty<string>();

        var findAllSearchPattern = string.IsNullOrEmpty(searchPattern) || searchPattern == "*";
        if (findAllSearchPattern)
            return files;

        var regexPattern = "^" + Regex.Escape(searchPattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

        return files.FindAll(file => regex.IsMatch(Path.GetFileName(file)));
    }

    public string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    public bool DirectoryExists(string path)
    {
        return Directories.ContainsKey(path);
    }

    public string GetFileContent(string filePath)
    {
        return string.Empty;
    }
}