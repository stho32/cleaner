using cleaner.Domain.FileBasedRules.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules;

public class RowLimitRuleTests
{
    [Test]
    public void FileUnderLimit_NoWarning()
    {
        var rule = new RowLimitRule(10);
        var source = "line1\nline2\nline3";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void FileOverLimit_Warning()
    {
        var rule = new RowLimitRule(5);
        var source = string.Join("\n", Enumerable.Range(1, 10).Select(i => $"// line {i}"));
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].ErrorMessage, Does.Contain("10 rows"));
        Assert.That(result[0].ErrorMessage, Does.Contain("allowed limit of 5"));
    }

    [Test]
    public void FileAtExactLimit_NoWarning()
    {
        var rule = new RowLimitRule(5);
        var source = string.Join("\n", Enumerable.Range(1, 5).Select(i => $"// line {i}"));
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void DefaultThreshold500_Respected()
    {
        var rule = new RowLimitRule();
        var source = string.Join("\n", Enumerable.Range(1, 501).Select(i => $"// line {i}"));
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Has.Length.EqualTo(1));
    }

    [Test]
    public void HandlesWindowsLineEndings()
    {
        var rule = new RowLimitRule(3);
        var source = "line1\r\nline2\r\nline3\r\nline4";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Has.Length.EqualTo(1));
    }
}
