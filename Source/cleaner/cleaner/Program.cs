using cleaner.Domain;
using cleaner.Domain.FileSystem;
using cleaner.Domain.Formatter;
using cleaner.Domain.Helpers;
using cleaner.Domain.Rules;

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

            _validationRules = new CompositeRule(
                new IRule[]
                {
                    new AllowedUsingsRule(allowedUsings),
                    new FileNameMatchingDeclarationRule(),
                    new ForEachDataSourceRule(),
                    new IfStatementDotsRule(),
                    new IfStatementOperatorRule(),
                    new LinqExpressionLengthRule(),
                    new MethodLengthRule(),
                    new NestedIfStatementsRule(),
                    new SqlInNonRepositoryRule(),
                    new NoConfigurationManagerRule(),
                    new NoOutAndRefParametersRule(),
                    new NoPublicFieldsRule(),
                    new NoPublicGenericListPropertiesRule(),
                    new NotImplementedExceptionRule(),
                    new PublicPropertiesPrivateSettersRule(),
                    new RepositoryConstructorRule(),
                    new RepositoryInheritanceRule(),
                    new RowLimitRule(),
                    new SingleDeclarationRule()
                });
            
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

            return GetDefaultAllowedUsings();
        }

        private static CommandLineArgumentParser ParseCommandLineArguments(string[] args)
        {
            var parser = new CommandLineArgumentParser(args);

            if (!parser.IsValid) 
                CommandLineArgumentParser.PrintUsage();

            return parser;
        }

        private static HashSet<string> GetDefaultAllowedUsings()
        {
            return new HashSet<string>
            {
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "Newtonsoft.Json",
                "Newtonsoft.Json.Linq",
                "System.Data",
                "System.Data.SqlClient",
                "System.Text",
                "System.ComponentModel",
                "System.Web",
                "System.Web.Mvc",
                "System.Web.Routing",
                "Newtonsoft.Json.Serialization",
                "WebGrease.Css.Extensions",
                "System.IO",
                "Microsoft.CodeAnalysis",
                "Microsoft.CodeAnalysis.CSharp",
                "Microsoft.CodeAnalysis.CSharp.Syntax",
                "System.Net.Http",
                "System.Threading",
                "System.Threading.Tasks",
                "System.Runtime.CompilerServices"
            };
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