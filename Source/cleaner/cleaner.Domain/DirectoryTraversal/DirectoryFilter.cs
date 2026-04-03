using cleaner.Domain.FileSystem;

namespace cleaner.Domain.DirectoryTraversal;

public static class DirectoryFilter
{
    public static bool ShouldIgnore(string subdirectory, IFileSystemAccessProvider fileSystemAccessProvider)
    {
        var isGeneratedFolderOnLinux = subdirectory.Contains("/bin/") || subdirectory.Contains("/obj/");
        if (isGeneratedFolderOnLinux)
            return true;

        var isGeneratedFolderOnWindows = subdirectory.Contains("\\bin\\") || subdirectory.Contains("\\obj\\");
        if (isGeneratedFolderOnWindows)
            return true;

        var lastFolderName = fileSystemAccessProvider.GetFileName(subdirectory);
        var lastFolderNameStartsWithADot = lastFolderName.StartsWith(".");

        if (lastFolderNameStartsWithADot)
            return true;

        return false;
    }
}
