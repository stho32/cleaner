using cleaner.Domain;
using cleaner.Domain.FileSystem;
using cleaner.Domain.Formatter;
using cleaner.Domain.Rules;

namespace cleaner
{
    public static class Program
    {
        private static IRule? _validationRules; 
        private static IFileSystemAccessProvider? _fileSystemAccessProvider;
        
        static void Main(string[] args)
        {
            var parser = new CommandLineArgumentParser(args);

            if (!parser.IsValid)
            {
                CommandLineArgumentParser.PrintUsage();
                return;
            }

            HashSet<string> allowedUsings;

            if (!string.IsNullOrEmpty(parser.AllowedUsingsFilePath))
            {
                try
                {
                    allowedUsings = new HashSet<string>(File.ReadAllLines(parser.AllowedUsingsFilePath));
                }
                catch (IOException e)
                {
                    Console.WriteLine($"Error reading allowed usings file: {e.Message}");
                    return;
                }
            }
            else
            {
                allowedUsings = GetDefaultAllowedUsings();
            }

            _validationRules = new CompositeRule(
                new IRule[]
                {
                    new AllowedUsingsRule(allowedUsings),
                    new FileNameMatchingDeclarationRule(),
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
            
            var fileContent = _fileSystemAccessProvider?.GetFileContent(filePath);

            if (string.IsNullOrWhiteSpace(fileContent)) 
                return false;
            
            _totalFilesChecked += 1;

            var messages = _validationRules?.Validate(filePath, fileContent);
            var messagePrinter = new ValidationMessagePrinter();

            if (messages != null && messages.Length > 0)
            {
                _totalFilesWithProblems += 1;
                _totalProblems += messages.Length;
                
                messagePrinter.Print(messages!);
                return true;
            }

            return false;
        }
    }
}