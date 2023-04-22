using cleaner.Domain;
using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.FileSystem;

namespace cleaner
{
    public static class Program
    {
        private static IFileSystemAccessProvider? _fileSystemAccessProvider;

        static void Main(string[] args)
        {
            var commandLineOptions = CommandLineArgumentParser.ParseCommandLineArguments(args);
            if (commandLineOptions == null)
                return;

            if (!string.IsNullOrWhiteSpace(commandLineOptions.DirectoryPath))
            {
                var qualityScanner = new QualityScanner();
                qualityScanner.PerformQualityScan(commandLineOptions);
            }
        }
    }
}