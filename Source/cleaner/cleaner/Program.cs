using cleaner.Domain;
using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.Rules;

namespace cleaner
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var commandLineOptions = CommandLineArgumentParser.ParseCommandLineArguments(args);
            if (commandLineOptions == null)
                return;

            if (commandLineOptions.ListRules)
            {
                ListAllRules();
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(commandLineOptions.DirectoryPath))
            {
                var qualityScanner = new QualityScanner();
                qualityScanner.PerformQualityScan(commandLineOptions);
                return;
            }
            
            Console.WriteLine("No directory path/action specified.");
        }

        private static void ListAllRules()
        {
            var rules = 
                RuleFactory.GetRules(AllowedUsingsRule.GetDefaultAllowedUsings(), "");

            Console.WriteLine("List of existing rules:");

            foreach (var rule in rules)
            {
                Console.WriteLine($"- {rule.Name}");
            }
        }
    }
}