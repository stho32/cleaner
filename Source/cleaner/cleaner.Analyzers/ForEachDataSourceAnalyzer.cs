using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace cleaner.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ForEachDataSourceAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CLEAN005";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "ForEach data source has too many dots",
        messageFormat: "ForEach data source contains more than 2 dots (Law of Demeter violation)",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "ForEach data source expressions should not have more than 2 dots, following the Law of Demeter.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    private const int MaxDots = 2;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeForEach, SyntaxKind.ForEachStatement);
    }

    private static void AnalyzeForEach(SyntaxNodeAnalysisContext context)
    {
        var forEachStatement = (ForEachStatementSyntax)context.Node;
        var dataSource = forEachStatement.Expression.ToString();
        var dotCount = dataSource.Split('.').Length - 1;

        if (dotCount > MaxDots)
        {
            var diagnostic = Diagnostic.Create(Rule, forEachStatement.Expression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
