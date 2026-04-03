using cleaner.Analyzers.Tests.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace cleaner.Analyzers.Tests;

[TestFixture]
public class NoOutRefParametersAnalyzerTests
{
    [Test]
    public async Task MethodWithoutOutOrRef_NoDiagnostic()
    {
        const string source = @"
public class TestClass
{
    public void TestMethod(int value)
    {
    }
}";

        await CSharpAnalyzerVerifier<NoOutRefParametersAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task MethodWithOutParameter_ReportsDiagnostic()
    {
        const string source = @"
public class TestClass
{
    public void TestMethod({|#0:out int value|})
    {
        value = 0;
    }
}";

        var expected = CSharpAnalyzerVerifier<NoOutRefParametersAnalyzer>
            .Diagnostic(NoOutRefParametersAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("value", "TestMethod", "out");

        await CSharpAnalyzerVerifier<NoOutRefParametersAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MethodWithRefParameter_ReportsDiagnostic()
    {
        const string source = @"
public class TestClass
{
    public void TestMethod({|#0:ref int value|})
    {
    }
}";

        var expected = CSharpAnalyzerVerifier<NoOutRefParametersAnalyzer>
            .Diagnostic(NoOutRefParametersAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("value", "TestMethod", "ref");

        await CSharpAnalyzerVerifier<NoOutRefParametersAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task OverrideMethodWithOutParameter_NoDiagnostic()
    {
        const string source = @"
public class BaseClass
{
    public virtual void TestMethod({|#0:out int value|}) { value = 0; }
}

public class TestClass : BaseClass
{
    public override void TestMethod(out int value)
    {
        value = 1;
    }
}";

        // Override methods are excluded; the base class method triggers the diagnostic
        var expected = CSharpAnalyzerVerifier<NoOutRefParametersAnalyzer>
            .Diagnostic(NoOutRefParametersAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("value", "TestMethod", "out");

        await CSharpAnalyzerVerifier<NoOutRefParametersAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }
}
