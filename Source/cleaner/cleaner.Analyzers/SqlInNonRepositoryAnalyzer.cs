using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace cleaner.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SqlInNonRepositoryAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CLEAN001";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "SQL in non-Repository class",
        messageFormat: "SQL detected in non-Repository class '{0}'",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "SQL strings should only appear in classes whose name ends with 'Repository'.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    private static readonly Regex SqlRegex = new Regex(
        @"\b(?:SELECT|TOP|INSERT|UPDATE|DELETE|FROM|WHERE|EXEC|ORDER BY)\b",
        RegexOptions.IgnoreCase);

    private const int SqlKeywordThreshold = 3;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var className = classDeclaration.Identifier.Text;

        if (className.EndsWith("Repository", System.StringComparison.OrdinalIgnoreCase))
            return;

        var stringLiterals = classDeclaration.DescendantNodes()
            .OfType<LiteralExpressionSyntax>()
            .Where(node => node.IsKind(SyntaxKind.StringLiteralExpression));

        foreach (var literal in stringLiterals)
        {
            var matches = SqlRegex.Matches(literal.Token.ValueText);
            if (matches.Count >= SqlKeywordThreshold)
            {
                var diagnostic = Diagnostic.Create(Rule, literal.GetLocation(), className);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
