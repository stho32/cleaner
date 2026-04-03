using CommandLine;
using CommandLine.Text;

namespace cleaner.Domain.CommandLineArguments;

public enum ParseResultType
{
    Success,
    ShowedHelp,
    ShowedVersion,
    Error
}

public class ParseResult
{
    public ParseResultType Type { get; }
    public CommandLineOptions? Options { get; }
    public string Output { get; }

    private ParseResult(ParseResultType type, CommandLineOptions? options, string output)
    {
        Type = type;
        Options = options;
        Output = output;
    }

    public static ParseResult Success(CommandLineOptions options) => new(ParseResultType.Success, options, "");
    public static ParseResult Help(string helpText) => new(ParseResultType.ShowedHelp, null, helpText);
    public static ParseResult Version(string version) => new(ParseResultType.ShowedVersion, null, version);
    public static ParseResult Error(string errorText) => new(ParseResultType.Error, null, errorText);
}

public static class CommandLineArgumentParser
{
    public static ParseResult ParseCommandLineArguments(string[] args)
    {
        var parser = new Parser(with => with.HelpWriter = null);
        var parserResult = parser.ParseArguments<CommandLineOptions>(args);

        ParseResult? result = null;

        parserResult
            .WithParsed(options => result = ParseResult.Success(options))
            .WithNotParsed(errors =>
            {
                var errorArray = errors as Error[] ?? errors.ToArray();

                if (errorArray.IsVersion())
                {
                    result = ParseResult.Version("Version 1.0.0");
                    return;
                }

                if (errorArray.IsHelp())
                {
                    result = ParseResult.Help(HelpText.AutoBuild(parserResult, h => h));
                    return;
                }

                result = ParseResult.Error(HelpText.AutoBuild(parserResult, h => h));
            });

        return result!;
    }
}
