using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace cleaner.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoOutRefParametersAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CLEAN006";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Method uses out or ref parameter",
        messageFormat: "Parameter '{0}' in '{1}' uses {2} modifier",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Methods and indexers should not use out or ref parameters.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeIndexer, SyntaxKind.IndexerDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (method.Modifiers.Any(SyntaxKind.OverrideKeyword))
            return;

        var methodName = method.Identifier.Text;
        CheckParameters(context, method.ParameterList, methodName);
    }

    private static void AnalyzeIndexer(SyntaxNodeAnalysisContext context)
    {
        var indexer = (IndexerDeclarationSyntax)context.Node;
        CheckParameters(context, indexer.ParameterList, "indexer");
    }

    private static void CheckParameters(SyntaxNodeAnalysisContext context, BaseParameterListSyntax parameterList, string memberName)
    {
        foreach (var parameter in parameterList.Parameters)
        {
            if (parameter.Modifiers.Any(SyntaxKind.RefKeyword))
            {
                var diagnostic = Diagnostic.Create(Rule, parameter.GetLocation(), parameter.Identifier.Text, memberName, "ref");
                context.ReportDiagnostic(diagnostic);
            }
            else if (parameter.Modifiers.Any(SyntaxKind.OutKeyword))
            {
                var diagnostic = Diagnostic.Create(Rule, parameter.GetLocation(), parameter.Identifier.Text, memberName, "out");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
