using cleaner.Domain.FileBasedRules.Rules;

namespace cleaner.Domain
{
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