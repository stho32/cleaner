using CommandLine;
using CommandLine.Text;

namespace cleaner.Domain.CommandLineArguments
{
    public static class CommandLineArgumentParser
    {
        public static CommandLineOptions? ParseCommandLineArguments(string[] args)
        {
            CommandLineOptions options = null;
            var parser = new Parser(with => with.HelpWriter = null); // Disable automatic help output

            var parserResult = parser.ParseArguments<CommandLineOptions>(args);

            parserResult
                .WithParsed(parsedOptions => options = parsedOptions)
                .WithNotParsed(errors =>
                {
                    if (errors.IsVersion())
                    {
                        Console.WriteLine("Version 1.0.0");
                        Environment.Exit(0);
                    }
                    if (errors.IsHelp())
                    {
                        Console.WriteLine(HelpText.AutoBuild(parserResult, h => h));
                        Environment.Exit(0);
                    }
                    Console.Error.WriteLine(HelpText.AutoBuild(parserResult, h => h));
                    Environment.Exit(1);
                });

            return options;
        }
    }
}