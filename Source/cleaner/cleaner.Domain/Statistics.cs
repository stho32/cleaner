using cleaner.Domain.FileBasedRules.Rules;

namespace cleaner.Domain
{
    public interface IStatisticsCollector
    {
        void FoundFileWithProblems();
        void ScanningFile();
        void ScanningFolder();
        void FoundFolderWithProblems();
    }

    public class StatisticsCollector : IStatisticsCollector
    {
        private int _filesWithProblems = 0;
        private int _foldersWithProblems = 0;
        private int _filesScanned = 0;
        private int _foldersScanned = 0;

        public void FoundFileWithProblems()
        {
            _filesWithProblems++;
        }

        public void ScanningFile()
        {
            _filesScanned++;
        }

        public void ScanningFolder()
        {
            _foldersScanned++;
        }

        public void FoundFolderWithProblems()
        {
            _foldersWithProblems++;
        }

        public Statistic GetStatistics()
        {
            return new Statistic(_filesWithProblems, _foldersWithProblems, _filesScanned, _foldersScanned);
        }
    }


    public record Statistic (int filesWithProblems, int foldersWithProblems, int filesScanned, int foldersScanned);


    public class Statistics
    {
        private readonly Statistic _statistic;
        public int TotalMessages { get; }
        public Dictionary<string, int> MessagesPerRule { get; }

        public Statistics(ValidationMessage[] messages, Statistic statistic)
        {
            _statistic = statistic;
            TotalMessages = messages.Length;
            MessagesPerRule = messages.GroupBy(m => m.RuleId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public void PrintStatistics()
        {
            Console.WriteLine($"Total messages: {TotalMessages}");
            Console.WriteLine("Messages per rule:");
            foreach (var (ruleId, count) in MessagesPerRule)
            {
                Console.WriteLine($" - Rule ID: {ruleId}, Count: {count}");
            }

            Console.WriteLine("");
            Console.WriteLine($"Files scanned: {_statistic.filesScanned}");
            Console.WriteLine($"Folders scanned: {_statistic.foldersScanned}");
            Console.WriteLine($"Files with problems: {_statistic.filesWithProblems}");
            Console.WriteLine($"Folders with problems: {_statistic.foldersWithProblems}");
        }
    }
}