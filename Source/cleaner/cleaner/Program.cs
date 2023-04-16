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
                    new SingleDeclarationRule(),
                    new NotImplementedExceptionRule()
                });
            _fileSystemAccessProvider = new FileSystemAccessProvider();

            var walker = new DirectoryWalker(ValidateRules, new FileSystemAccessProvider(), "*.cs");
            walker.Walk(result);
        }

        private static void ValidateRules(string filePath)
        {
            var fileContent = _fileSystemAccessProvider?.GetFileContent(filePath);

            if (fileContent != null)
            {
                var messages = _validationRules?.Validate(filePath, fileContent);
                var messagePrinter = new ValidationMessagePrinter();

                if (messages != null) 
                    messagePrinter.Print(messages);
            }
        }
    }
}