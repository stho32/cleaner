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
            var cmdParser = new CommandLineArgumentParser();

            var result = cmdParser.Parse(args);
            if (string.IsNullOrWhiteSpace(result))
                return;

            _validationRules = new CompositeRule(
                new IRule[]
                {
                    new AllowedUsingsRule(
                        new HashSet<string>
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
                            "WebGrease.Css.Extensions"
                        }),
                    new FileNameMatchingDeclarationRule(),
                    new IfStatementOperatorRule(),
                    new LinqExpressionLengthRule(),
                    new MethodLengthRule(),
                    new SqlInNonRepositoryRule(),
                    new NoConfigurationManagerRule(),
                    new NoOutAndRefParametersRule(),
                    new NoPublicFieldsRule(),
                    new NoPublicGenericListPropertiesRule(),
                    new NotImplementedExceptionRule(),
                    new PublicPropertiesPrivateSettersRule(),
                    new RowLimitRule(),
                    new SingleDeclarationRule()
                });
            _fileSystemAccessProvider = new FileSystemAccessProvider();

            var walker = new DirectoryWalker(ValidateRules, new FileSystemAccessProvider(), "*.cs");
            walker.Walk(result);
        }

        private static bool IsDesignerFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            return fileName.Contains(".Designer.");
        }

        private static bool ValidateRules(string filePath)
        {
            if (IsDesignerFile(filePath))
                return false;
            
            var fileContent = _fileSystemAccessProvider?.GetFileContent(filePath);

            if (string.IsNullOrWhiteSpace(fileContent)) 
                return false;
            
            var messages = _validationRules?.Validate(filePath, fileContent);
            var messagePrinter = new ValidationMessagePrinter();

            if (messages != null && messages.Length > 0)
            {
                messagePrinter.Print(messages);
                return true;
            }

            return false;
        }
    }
}