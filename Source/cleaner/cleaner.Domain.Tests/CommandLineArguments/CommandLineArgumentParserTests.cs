using cleaner.Domain.CommandLineArguments;
using NUnit.Framework;

namespace cleaner.Domain.Tests.CommandLineArguments;

public class CommandLineArgumentParserTests
{
    [Test]
    public void Parse_DirectoryOption_ReturnsSuccess()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "-d", "/tmp" });

        Assert.That(result.Type, Is.EqualTo(ParseResultType.Success));
        Assert.That(result.Options, Is.Not.Null);
        Assert.That(result.Options!.DirectoryPath, Is.EqualTo("/tmp"));
    }

    [Test]
    public void Parse_ListRulesOption_ReturnsSuccess()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "-r" });

        Assert.That(result.Type, Is.EqualTo(ParseResultType.Success));
        Assert.That(result.Options!.ListRules, Is.True);
    }

    [Test]
    public void Parse_HelpOption_ReturnsHelp()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "--help" });

        Assert.That(result.Type, Is.EqualTo(ParseResultType.ShowedHelp));
        Assert.That(result.Output, Does.Contain("cleaner"));
    }

    [Test]
    public void Parse_VersionOption_ReturnsVersion()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "--version" });

        Assert.That(result.Type, Is.EqualTo(ParseResultType.ShowedVersion));
        Assert.That(result.Output, Does.Contain("Version"));
    }

    [Test]
    public void Parse_InvalidOption_ReturnsError()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "--nonexistent" });

        Assert.That(result.Type, Is.EqualTo(ParseResultType.Error));
        Assert.That(result.Output, Is.Not.Empty);
    }

    [Test]
    public void Parse_StopOnFirst_SetsFlag()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(new[] { "-d", "/tmp", "-s" });

        Assert.That(result.Options!.StopOnFirstFileWithProblems, Is.True);
    }

    [Test]
    public void Parse_LatestChangedFiles_SetsValue()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(
            new[] { "-d", "/tmp", "--latestChangedFiles", "10" });

        Assert.That(result.Options!.LatestChangedFiles, Is.EqualTo(10));
    }

    [Test]
    public void Parse_AllowedUsings_SetsPath()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(
            new[] { "-d", "/tmp", "--allowedUsings", "usings.txt" });

        Assert.That(result.Options!.AllowedUsingsFilePath, Is.EqualTo("usings.txt"));
    }

    [Test]
    public void Parse_NoArgs_ReturnsSuccess()
    {
        var result = CommandLineArgumentParser.ParseCommandLineArguments(Array.Empty<string>());

        Assert.That(result.Type, Is.EqualTo(ParseResultType.Success));
    }
}
