using System.Reflection;
using cleaner.Domain;
using cleaner.Domain.FileSystem;
using cleaner.Domain.Formatter;
using cleaner.Domain.Helpers;
using cleaner.Domain.Rules;
using cleaner.Domain.Rules.NestedIfStatementsRuleValidation;

namespace cleaner
{
    public static class Program
    {
        private static IRule? _validationRules; 
        private static IFileSystemAccessProvider? _fileSystemAccessProvider;
        
        static void Main(string[] args)
        {
            var parser = ParseCommandLineArguments(args); 
            if (!parser.IsValid) 
                return;

            HashSet<string> allowedUsings = LoadAllowedUsingsOrUseDefault(parser.AllowedUsingsFilePath);

            // Dynamically load all rule types
            var ruleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IRule)) && !t.IsAbstract);

            // Create rule instances
            var ruleInstances = new List<IRule>();
            foreach (var ruleType in ruleTypes)
            {
                if (ruleType == typeof(AllowedUsingsRule))
                {
                    ruleInstances.Add(new AllowedUsingsRule(allowedUsings));
                }
                else
                {
                    ruleInstances.Add((IRule)Activator.CreateInstance(ruleType));
                }
            }

            _validationRules = new CompositeRule(ruleInstances);
            
            _fileSystemAccessProvider = new FileSystemAccessProvider();

            var walker = new DirectoryWalker(ValidateRules, new FileSystemAccessProvider(), "*.cs");
            walker.Walk(parser.DirectoryPath??"", parser.StopOnFirstFileWithProblems);

            PrintStatistics(_totalFilesChecked, _totalFilesWithProblems, _totalProblems);
        }

        private static HashSet<string> LoadAllowedUsingsOrUseDefault(string? allowedUsingsFilePath)
        {
            if (!string.IsNullOrEmpty(allowedUsingsFilePath))
            {
                try
                {
                    return new HashSet<string>(File.ReadAllLines(allowedUsingsFilePath));
                }
                catch (IOException e)
                {
                    Console.WriteLine($"Error reading allowed usings file: {e.Message}");
                }
            }

            return AllowedUsingsRule.GetDefaultAllowedUsings();
        }

        private static CommandLineArgumentParser ParseCommandLineArguments(string[] args)
        {
            var parser = new CommandLineArgumentParser(args);

            if (!parser.IsValid) 
                CommandLineArgumentParser.PrintUsage();

            return parser;
        }

        
        private static bool IsDesignerFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            return fileName.Contains(".Designer.");
        }

        private static int _totalFilesChecked = 0;
        private static int _totalFilesWithProblems = 0;
        private static int _totalProblems = 0;
        
        private static void PrintStatistics(int totalFilesChecked, int totalFilesWithProblems, int totalProblems)
        {
            Console.WriteLine("\nStatistics:");
            Console.WriteLine($"  Total files checked:       {totalFilesChecked}");
            Console.WriteLine($"  Total files with problems: {totalFilesWithProblems}");
            Console.WriteLine($"  Total problems found:      {totalProblems}");
        }
        
        private static bool ValidateRules(string filePath)
        {
            if (IsDesignerFile(filePath))
                return false;

            string? fileContent = GetFileContent(filePath);
            if (string.IsNullOrWhiteSpace(fileContent))
                return false;

            IncrementTotalFilesChecked();

            ValidationMessage[]? messages = RunValidationRules(filePath, fileContent);
            if (CollectionHelpers.IsNullOrEmpty(messages))
                return false;

            UpdateProblemStatistics(messages);
            PrintValidationMessages(messages);

            return true;
        }

        private static string? GetFileContent(string filePath)
        {
            return _fileSystemAccessProvider?.GetFileContent(filePath);
        }

        private static void IncrementTotalFilesChecked()
        {
            _totalFilesChecked += 1;
        }

        private static ValidationMessage[]? RunValidationRules(string filePath, string fileContent)
        {
            var messages = _validationRules!.Validate(filePath, fileContent);
            return messages;
        }

        private static void UpdateProblemStatistics(ValidationMessage[]? messages)
        {
            if (messages == null)
                return;
            
            _totalFilesWithProblems += 1;
            _totalProblems += messages.Length;
        }

        private static void PrintValidationMessages(ValidationMessage[]? messages)
        {
            var messagePrinter = new ValidationMessagePrinter();
            messagePrinter.Print(messages);
        }
    }
}