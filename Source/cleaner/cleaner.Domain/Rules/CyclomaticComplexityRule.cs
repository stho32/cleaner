using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class CyclomaticComplexityRule : IRule
{
    public string Id => "CyclomaticComplexityRule";
    public string Name => "Cyclomatic Complexity Rule";
    public string ShortDescription => "Warns when cyclomatic complexity exceeds a given threshold.";

    public string LongDescription =>
        "This rule calculates the cyclomatic complexity of each method and warns when it exceeds a given threshold. Cyclomatic complexity is a metric that measures the number of linearly independent paths through a program's source code.";

    private readonly int _threshold = 5;

    public ValidationMessage[]? Validate(string filePath, string fileContent)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        var messages = new List<ValidationMessage>();

        foreach (var method in methods)
        {
            int complexity = CalculateCyclomaticComplexity(method);
            var complexityExceedsThresold = complexity > _threshold;
            
            if (complexityExceedsThresold)
            {
                int lineNumber = method.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                messages.Add(new ValidationMessage(
                    Id,
                    Name,
                    $"Method '{method.Identifier.ValueText}' in file '{filePath}' at line {lineNumber} has a cyclomatic complexity of {complexity}, which exceeds the threshold of {_threshold}."));           }
        }

        return messages.ToArray();
    }

    private int CalculateCyclomaticComplexity(MethodDeclarationSyntax method)
    {
        var decisionPoints = method.DescendantNodes().OfType<SyntaxNode>()
            .Count(node => node is IfStatementSyntax
                           || node is WhileStatementSyntax
                           || node is ForStatementSyntax
                           || node is ForEachStatementSyntax
                           || node is CaseSwitchLabelSyntax
                           || node is DefaultSwitchLabelSyntax
                           || node is CatchClauseSyntax
                           || node is ConditionalExpressionSyntax);

        return decisionPoints + 1;
    }
}