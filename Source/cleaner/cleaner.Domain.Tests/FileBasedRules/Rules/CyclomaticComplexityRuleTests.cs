using cleaner.Domain.FileBasedRules.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules;

public class CyclomaticComplexityRuleTests
{
    private CyclomaticComplexityRule _rule = null!;

    [SetUp]
    public void Setup()
    {
        _rule = new CyclomaticComplexityRule(4);
    }

    [Test]
    public void SimpleMethod_NoWarning()
    {
        var source = @"
public class Foo
{
    public void Bar() { int x = 1; }
}";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = _rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void MethodWithHighComplexity_Warning()
    {
        var source = @"
public class Foo
{
    public void Bar(int x)
    {
        if (x > 0) { }
        if (x > 1) { }
        if (x > 2) { }
        if (x > 3) { }
        if (x > 4) { }
    }
}";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = _rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].RuleId, Is.EqualTo("CyclomaticComplexityRule"));
        Assert.That(result[0].ErrorMessage, Does.Contain("cyclomatic complexity of 6"));
    }

    [Test]
    public void MethodAtExactThreshold_NoWarning()
    {
        var source = @"
public class Foo
{
    public void Bar(int x)
    {
        if (x > 0) { }
        if (x > 1) { }
        if (x > 2) { }
    }
}";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = _rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void CustomThreshold_Respected()
    {
        var rule = new CyclomaticComplexityRule(1);
        var source = @"
public class Foo
{
    public void Bar(int x)
    {
        if (x > 0) { }
        if (x > 1) { }
    }
}";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Has.Length.EqualTo(1));
    }

    [Test]
    public void CountsForEachWhileForSwitchCatch()
    {
        var source = @"
using System.Collections.Generic;
public class Foo
{
    public void Bar(int x, List<int> items)
    {
        while (x > 0) { x--; }
        for (int i = 0; i < 10; i++) { }
        foreach (var item in items) { }
        try { } catch (System.Exception) { }
        var y = x > 0 ? 1 : 0;
    }
}";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = _rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].ErrorMessage, Does.Contain("cyclomatic complexity of 6"));
    }

    [Test]
    public void NoMethods_NoWarning()
    {
        var source = "public class Foo { public int X { get; set; } }";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetCompilationUnitRoot();

        var result = _rule.Validate("test.cs", source, tree, root);

        Assert.That(result, Is.Empty);
    }
}
