using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace cleaner.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IfStatementDotsAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CLEAN004";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "If condition has too many dots",
        messageFormat: "If condition contains expressions with more than 2 dots (Law of Demeter violation)",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "If conditions should not have expressions with more than 2 dots, following the Law of Demeter.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    private const int MaxDots = 2;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
    }

    private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
    {
        var ifStatement = (IfStatementSyntax)context.Node;

        var hasTooManyDots = ifStatement.Condition.DescendantNodesAndSelf()
            .Any(n => (n.ToString().Split('.').Length - 1) > MaxDots);

        if (hasTooManyDots)
        {
            var diagnostic = Diagnostic.Create(Rule, ifStatement.IfKeyword.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
