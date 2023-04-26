using cleaner.Domain;
using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;

namespace cleaner
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var commandLineOptions = CommandLineArgumentParser.ParseCommandLineArguments(args);
            if (commandLineOptions == null)
                return;

            if (ListRules(commandLineOptions)) return;

            var directoryWalker = GetMatchingDirectoryWalker(commandLineOptions);
            
            if (ScanFiles(commandLineOptions, directoryWalker)) return;

            Console.WriteLine("No directory path/action specified.");
        }

        private static IDirectoryWalker GetMatchingDirectoryWalker(CommandLineOptions commandLineOptions)
        {
            if (commandLineOptions.LatestChangedFiles == null)
                return new RecursiveDirectoryWalker();

            return new LatestChangedFilesDirectoryWalker(commandLineOptions.LatestChangedFiles.Value);
        }

        private static bool ScanFiles(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker)
        {
            if (!string.IsNullOrWhiteSpace(commandLineOptions.DirectoryPath))
            {
                var qualityScanner = new QualityScanner();
                qualityScanner.PerformQualityScan(commandLineOptions, directoryWalker);
                return true;
            }

            return false;
        }

        private static bool ListRules(CommandLineOptions commandLineOptions)
        {
            if (commandLineOptions.ListRules)
            {
                var ruleLister = new RuleLister();
                ruleLister.ListAllRules();
                return true;
            }

            return false;
        }
    }
}