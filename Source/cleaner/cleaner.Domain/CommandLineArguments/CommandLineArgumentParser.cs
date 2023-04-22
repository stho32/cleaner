using CommandLine;
using CommandLine.Text;

namespace cleaner.Domain.CommandLineArguments
{
    public static class CommandLineArgumentParser
    {
        public static CommandLineOptions? ParseCommandLineArguments(string[] args)
        {
            CommandLineOptions options = null!;
            var parser = new Parser(with => with.HelpWriter = null); // Disable automatic help output

            var parserResult = parser.ParseArguments<CommandLineOptions>(args);

            parserResult
                .WithParsed(parsedOptions => options = parsedOptions)
                .WithNotParsed(errors =>
                {
                    var enumerable = errors as Error[] ?? errors.ToArray();
                    
                    if (enumerable.IsVersion())
                    {
                        PrintVersionAndExit();
                    }
                    if (enumerable.IsHelp())
                    {
                        PrintHelptextAndExit(parserResult);
                    }
                    Console.Error.WriteLine(HelpText.AutoBuild(parserResult, h => h));
                    Environment.Exit(1);
                });

            return options;
        }

        private static void PrintHelptextAndExit(ParserResult<CommandLineOptions> parserResult)
        {
            Console.WriteLine(HelpText.AutoBuild(parserResult, h => h));
            Environment.Exit(0);
        }

        private static void PrintVersionAndExit()
        {
            Console.WriteLine("Version 1.0.0");
            Environment.Exit(0);
        }
    }
}