using cleaner.Domain;
using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.Configuration;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules;
using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.FolderBasedRules;
using cleaner.Domain.Formatter;

namespace cleaner;

public static class Program
{
    static int Main(string[] args)
    {
        var parseResult = CommandLineArgumentParser.ParseCommandLineArguments(args);

        switch (parseResult.Type)
        {
            case ParseResultType.ShowedHelp:
            case ParseResultType.ShowedVersion:
                Console.WriteLine(parseResult.Output);
                return 0;

            case ParseResultType.Error:
                Console.Error.WriteLine(parseResult.Output);
                return 1;

            case ParseResultType.Success:
                return Run(parseResult.Options!);
        }

        return 1;
    }

    private static int Run(CommandLineOptions commandLineOptions)
    {
        if (commandLineOptions.ListRules)
        {
            var ruleLister = new RuleLister();
            ruleLister.ListAllRules();
            return 0;
        }

        if (!string.IsNullOrWhiteSpace(commandLineOptions.DirectoryPath))
        {
            var statisticsCollector = new StatisticsCollector();
            var config = CleanerConfigLoader.Load(commandLineOptions.DirectoryPath);

            var qualityScanner = new CompositeQualityScanner(
                new FolderBasedQualityScanner(config.MaxFilesPerDirectory, statisticsCollector),
                new FileBasedQualityScanner(statisticsCollector)
            );

            var directoryWalker = GetMatchingDirectoryWalker(commandLineOptions);
            var messages = qualityScanner.PerformQualityScan(commandLineOptions, directoryWalker);

            var messagePrinter = new ValidationMessagePrinter();
            messagePrinter.Print(messages);

            var statistics = new Statistics(messages, statisticsCollector.GetStatistics());
            statistics.PrintStatistics();

            return 0;
        }

        Console.WriteLine("No directory path/action specified.");
        return 1;
    }

    private static IDirectoryWalker GetMatchingDirectoryWalker(CommandLineOptions commandLineOptions)
    {
        if (commandLineOptions.LatestChangedFiles == null)
            return new RecursiveDirectoryWalker();

        return new LatestChangedFilesDirectoryWalker(commandLineOptions.LatestChangedFiles.Value);
    }
}
