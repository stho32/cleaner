using cleaner.Analyzers.Tests.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace cleaner.Analyzers.Tests;

[TestFixture]
public class IfStatementDotsAnalyzerTests
{
    [Test]
    public async Task IfWithTwoDotsOrLess_NoDiagnostic()
    {
        const string source = @"
public class TestClass
{
    public void TestMethod()
    {
        var obj = new System.Text.StringBuilder();
        if (obj.Length > 0)
        {
        }
    }
}";

        await CSharpAnalyzerVerifier<IfStatementDotsAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task IfWithMoreThanTwoDots_ReportsDiagnostic()
    {
        const string source = @"
public class Inner { public int Value { get; set; } }
public class Middle { public Inner Inner { get; set; } }
public class Outer { public Middle Middle { get; set; } }

public class TestClass
{
    public void TestMethod()
    {
        var outer = new Outer();
        {|#0:if|} (outer.Middle.Inner.Value > 0)
        {
        }
    }
}";

        var expected = CSharpAnalyzerVerifier<IfStatementDotsAnalyzer>
            .Diagnostic(IfStatementDotsAnalyzer.DiagnosticId)
            .WithLocation(0);

        await CSharpAnalyzerVerifier<IfStatementDotsAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }
}
