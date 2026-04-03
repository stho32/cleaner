using cleaner.Analyzers.Tests.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace cleaner.Analyzers.Tests;

[TestFixture]
public class IfStatementOperatorAnalyzerTests
{
    [Test]
    public async Task IfWithMethodCall_NoDiagnostic()
    {
        const string source = @"
public class TestClass
{
    public void TestMethod()
    {
        if (IsValid())
        {
        }
    }

    private bool IsValid() => true;
}";

        await CSharpAnalyzerVerifier<IfStatementOperatorAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task IfWithNullComparison_NoDiagnostic()
    {
        const string source = @"
public class TestClass
{
    public void TestMethod(object obj)
    {
        if (obj == null)
        {
        }
    }
}";

        await CSharpAnalyzerVerifier<IfStatementOperatorAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task IfWithBinaryOperator_ReportsDiagnostic()
    {
        const string source = @"
public class TestClass
{
    public void TestMethod(int a, int b)
    {
        {|#0:if|} (a > b)
        {
        }
    }
}";

        var expected = CSharpAnalyzerVerifier<IfStatementOperatorAnalyzer>
            .Diagnostic(IfStatementOperatorAnalyzer.DiagnosticId)
            .WithLocation(0);

        await CSharpAnalyzerVerifier<IfStatementOperatorAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }
}
