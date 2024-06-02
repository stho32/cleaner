namespace cleaner.Domain.FileSystem;

public class CSharpCodeFileFilter : IFileFilter
{
    public bool IsValidFilename(string filePath)
    {
        return IsCSharpFile(filePath) && !IsDesignerFile(filePath);
    }
    
    public bool IsValidContent(string? fileContent)
    {
        return !string.IsNullOrEmpty(fileContent);
    }

    private static bool IsCSharpFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return extension == ".cs";
    }

    private static bool IsDesignerFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        return fileName.Contains(".Designer.");
    }

}
