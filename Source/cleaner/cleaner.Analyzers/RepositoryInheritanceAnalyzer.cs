using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace cleaner.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RepositoryInheritanceAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CLEAN002";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Repository class inherits from another class",
        messageFormat: "Repository class '{0}' should not inherit from another class",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Repository classes should not inherit from other classes. Implementing interfaces is allowed.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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

        if (!className.EndsWith("Repository", System.StringComparison.OrdinalIgnoreCase))
            return;

        if (classDeclaration.BaseList == null)
            return;

        var inheritsFromClass = classDeclaration.BaseList.Types.Any(baseType =>
        {
            var typeName = baseType.Type switch
            {
                IdentifierNameSyntax ins => ins.Identifier.Text,
                QualifiedNameSyntax qns => qns.Right.Identifier.Text,
                _ => null
            };

            return typeName != null && !typeName.StartsWith("I", System.StringComparison.Ordinal);
        });

        if (inheritsFromClass)
        {
            var diagnostic = Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), className);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
