using cleaner.Analyzers.Tests.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace cleaner.Analyzers.Tests;

[TestFixture]
public class ForEachDataSourceAnalyzerTests
{
    [Test]
    public async Task ForEachWithTwoDotsOrLess_NoDiagnostic()
    {
        const string source = @"
using System.Collections.Generic;

public class Container { public List<string> Items { get; set; } }

public class TestClass
{
    public void TestMethod()
    {
        var container = new Container();
        foreach (var item in container.Items)
        {
        }
    }
}";

        await CSharpAnalyzerVerifier<ForEachDataSourceAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task ForEachWithMoreThanTwoDots_ReportsDiagnostic()
    {
        const string source = @"
using System.Collections.Generic;

public class Inner { public List<string> Items { get; set; } }
public class Middle { public Inner Inner { get; set; } }
public class Outer { public Middle Middle { get; set; } }

public class TestClass
{
    public void TestMethod()
    {
        var outer = new Outer();
        foreach (var item in {|#0:outer.Middle.Inner.Items|})
        {
        }
    }
}";

        var expected = CSharpAnalyzerVerifier<ForEachDataSourceAnalyzer>
            .Diagnostic(ForEachDataSourceAnalyzer.DiagnosticId)
            .WithLocation(0);

        await CSharpAnalyzerVerifier<ForEachDataSourceAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }
}
