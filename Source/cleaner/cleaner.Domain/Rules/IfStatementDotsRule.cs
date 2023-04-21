using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class IfStatementDotsRule : IRule
{
    public string Id => "IfStatementDotsRule";
    public string Name => "If Statement Dots Rule";
    public string ShortDescription => "Detects if statements with expressions containing more than two dots";
    public string LongDescription => "This rule checks if an if statement contains expressions with more than two dots and raises a warning if it does.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var ifStatements = root.DescendantNodes()
            .OfType<IfStatementSyntax>()
            .Where(IfStatementContainsTooManyDots)
            .ToArray();

        foreach (var ifStatement in ifStatements)
        {
            FileLinePositionSpan span = ifStatement.SyntaxTree.GetLineSpan(ifStatement.Span);
            int lineNumber = span.StartLinePosition.Line + 1;

            var message = new ValidationMessage(
                Severity.Warning,
                Id,
                Name,
                $"An if statement in the file '{filePath}':{lineNumber} contains expressions with more than two dots, which is not allowed."
            );
            messages.Add(message);
        }

        return messages.ToArray();
    }

    private bool IfStatementContainsTooManyDots(IfStatementSyntax ifStatement)
    {
        var nodes = ifStatement.Condition.DescendantNodesAndSelf();
        return nodes.Any(n => n.ToString().Count(c => c == '.') > 2);
    }
}