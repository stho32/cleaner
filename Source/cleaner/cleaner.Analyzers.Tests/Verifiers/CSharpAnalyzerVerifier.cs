using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace cleaner.Analyzers.Tests.Verifiers;

public class DefaultVerifier : IVerifier
{
    public void Empty<T>(string collectionName, IEnumerable<T> collection)
    {
        var items = collection.ToList();
        if (items.Count != 0)
            throw new InvalidOperationException($"Expected '{collectionName}' to be empty, but it contained {items.Count} items.");
    }

    public void Equal<T>(T expected, T actual, string? message = null)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException(message ?? $"Expected '{expected}', but got '{actual}'.");
    }

    public void True(bool assert, string? message = null)
    {
        if (!assert)
            throw new InvalidOperationException(message ?? "Expected true, but got false.");
    }

    public void False(bool assert, string? message = null)
    {
        if (assert)
            throw new InvalidOperationException(message ?? "Expected false, but got true.");
    }

    [DoesNotReturn]
    public void Fail(string? message = null)
    {
        throw new InvalidOperationException(message ?? "Verification failed.");
    }

    public void LanguageIsSupported(string language)
    {
        if (language != LanguageNames.CSharp)
            throw new InvalidOperationException($"Language '{language}' is not supported.");
    }

    public void NotEmpty<T>(string collectionName, IEnumerable<T> collection)
    {
        if (!collection.Any())
            throw new InvalidOperationException($"Expected '{collectionName}' to not be empty.");
    }

    public void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T>? equalityComparer = null, string? message = null)
    {
        var comparer = equalityComparer ?? EqualityComparer<T>.Default;
        if (!expected.SequenceEqual(actual, comparer))
            throw new InvalidOperationException(message ?? "Sequences are not equal.");
    }

    public IVerifier PushContext(string context) => this;
}

public static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => new DiagnosticResult(diagnosticId, DiagnosticSeverity.Warning);

    public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }
}
