using cleaner.Analyzers.Tests.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace cleaner.Analyzers.Tests;

[TestFixture]
public class SqlInNonRepositoryAnalyzerTests
{
    [Test]
    public async Task SqlInRepositoryClass_NoDiagnostic()
    {
        const string source = @"
public class UserRepository
{
    private string query = ""SELECT TOP 1 Id FROM Users WHERE Name = 'test' ORDER BY Id"";
}";

        await CSharpAnalyzerVerifier<SqlInNonRepositoryAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task SqlInNonRepositoryClass_ReportsDiagnostic()
    {
        const string source = @"
public class UserService
{
    private string query = {|#0:""SELECT TOP 1 Id FROM Users WHERE Name = 'test' ORDER BY Id""|};
}";

        var expected = CSharpAnalyzerVerifier<SqlInNonRepositoryAnalyzer>
            .Diagnostic(SqlInNonRepositoryAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("UserService");

        await CSharpAnalyzerVerifier<SqlInNonRepositoryAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task NonSqlStringInNonRepositoryClass_NoDiagnostic()
    {
        const string source = @"
public class UserService
{
    private string greeting = ""Hello, World!"";
}";

        await CSharpAnalyzerVerifier<SqlInNonRepositoryAnalyzer>.VerifyAnalyzerAsync(source);
    }
}
