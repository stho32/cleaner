using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules;
using cleaner.Domain.FileSystem;
using cleaner.Domain.Tests.Mocks;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules;

public class FileBasedQualityScannerTests
{
    private class InMemoryDirectoryWalker : IDirectoryWalker
    {
        private readonly string[] _files;
        public InMemoryDirectoryWalker(params string[] files) => _files = files;

        public void Walk(Func<string, bool> fileCallback, IFileSystemAccessProvider fileSystemAccessProvider,
            string searchPattern, string directoryPath, bool stopOnFirstFileWithErrors = false)
        {
            foreach (var file in _files)
            {
                var hasErrors = fileCallback(file);
                if (hasErrors && stopOnFirstFileWithErrors) return;
            }
        }
    }

    private class InMemoryFileSystem : MockFileSystemAccessProvider
    {
        private readonly Dictionary<string, string> _fileContents = new();
        public void AddFile(string path, string content) => _fileContents[path] = content;
        public new string GetFileContent(string filePath) =>
            _fileContents.TryGetValue(filePath, out var c) ? c : "";
    }

    [Test]
    public void ScanEmptyDirectory_NoMessages()
    {
        var collector = new StatisticsCollector();
        var walker = new InMemoryDirectoryWalker();
        var scanner = new FileBasedQualityScanner(collector);
        var options = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "-d", "." });

        var result = scanner.PerformQualityScan(options.Options!, walker);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ScanNonCsFile_Skipped()
    {
        var collector = new StatisticsCollector();
        var walker = new InMemoryDirectoryWalker("readme.md");
        var scanner = new FileBasedQualityScanner(collector);
        var options = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "-d", "." });

        var result = scanner.PerformQualityScan(options.Options!, walker);

        Assert.That(result, Is.Empty);
        Assert.That(collector.GetStatistics().filesScanned, Is.EqualTo(0));
    }

    [Test]
    public void ScanDesignerFile_Skipped()
    {
        var collector = new StatisticsCollector();
        var walker = new InMemoryDirectoryWalker("Form1.Designer.cs");
        var scanner = new FileBasedQualityScanner(collector);
        var options = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "-d", "." });

        var result = scanner.PerformQualityScan(options.Options!, walker);

        Assert.That(result, Is.Empty);
    }
}
