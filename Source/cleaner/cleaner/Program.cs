using cleaner.Domain;
using cleaner.Domain.CommandLineArguments;

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

            if (ScanFiles(commandLineOptions)) return;

            Console.WriteLine("No directory path/action specified.");
        }

        private static bool ScanFiles(CommandLineOptions commandLineOptions)
        {
            if (!string.IsNullOrWhiteSpace(commandLineOptions.DirectoryPath))
            {
                var qualityScanner = new QualityScanner();
                qualityScanner.PerformQualityScan(commandLineOptions);
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