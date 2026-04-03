using cleaner.Analyzers.Tests.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace cleaner.Analyzers.Tests;

[TestFixture]
public class RepositoryInheritanceAnalyzerTests
{
    [Test]
    public async Task RepositoryWithoutInheritance_NoDiagnostic()
    {
        const string source = @"
public class UserRepository
{
}";

        await CSharpAnalyzerVerifier<RepositoryInheritanceAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task RepositoryImplementingInterface_NoDiagnostic()
    {
        const string source = @"
public interface IUserRepository { }

public class UserRepository : IUserRepository
{
}";

        await CSharpAnalyzerVerifier<RepositoryInheritanceAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task RepositoryInheritingFromClass_ReportsDiagnostic()
    {
        const string source = @"
public class BaseRepository { }

public class {|#0:UserRepository|} : BaseRepository
{
}";

        var expected = CSharpAnalyzerVerifier<RepositoryInheritanceAnalyzer>
            .Diagnostic(RepositoryInheritanceAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("UserRepository");

        await CSharpAnalyzerVerifier<RepositoryInheritanceAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task NonRepositoryClassInheriting_NoDiagnostic()
    {
        const string source = @"
public class BaseService { }

public class UserService : BaseService
{
}";

        await CSharpAnalyzerVerifier<RepositoryInheritanceAnalyzer>.VerifyAnalyzerAsync(source);
    }
}
