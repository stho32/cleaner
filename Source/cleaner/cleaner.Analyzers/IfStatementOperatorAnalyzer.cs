using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace cleaner.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IfStatementOperatorAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CLEAN003";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "If statement contains operators",
        messageFormat: "If statement contains operators (IOSP violation)",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "If statements should not contain operators. This follows the Integration Operation Segregation Principle (IOSP).");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
    }

    private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
    {
        var ifStatement = (IfStatementSyntax)context.Node;

        var hasOperator = ifStatement.Condition.DescendantNodesAndSelf().Any(n =>
            (n is BinaryExpressionSyntax binaryExpression && !IsSimpleNullComparison(binaryExpression)) ||
            n is PostfixUnaryExpressionSyntax);

        if (hasOperator)
        {
            var diagnostic = Diagnostic.Create(Rule, ifStatement.IfKeyword.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsSimpleNullComparison(BinaryExpressionSyntax binaryExpression)
    {
        return (binaryExpression.Left is LiteralExpressionSyntax leftLiteral &&
                leftLiteral.IsKind(SyntaxKind.NullLiteralExpression)) ||
               (binaryExpression.Right is LiteralExpressionSyntax rightLiteral &&
                rightLiteral.IsKind(SyntaxKind.NullLiteralExpression));
    }
}
