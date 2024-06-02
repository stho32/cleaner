namespace cleaner.Domain.FileSystem;

public interface IFileFilter
{
    bool IsValidFilename(string filePath);

    bool IsValidContent(string? fileContent);
}