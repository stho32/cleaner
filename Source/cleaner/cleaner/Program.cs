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
                            // Add more allowed usings here
                        }),
                    new FileNameMatchingDeclarationRule(),
                    new IfStatementOperatorRule(),
                    new LinqExpressionLengthRule(),
                    new MethodLengthRule(),
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

        private static void ValidateRules(string filePath)
        {
            if (IsDesignerFile(filePath))
                return;
            
            var fileContent = _fileSystemAccessProvider?.GetFileContent(filePath);

            if (string.IsNullOrWhiteSpace(fileContent)) 
                return;
            
            var messages = _validationRules?.Validate(filePath, fileContent);
            var messagePrinter = new ValidationMessagePrinter();

            if (messages != null) 
                messagePrinter.Print(messages);
        }
    }
}