namespace cleaner.Domain.Rules;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class IfStatementOperatorRule : IRule
{
    public string Id => GetType().Name;
    public string Name => "If Statement Operator Rule";
    public string ShortDescription => "Detects if statements containing operators, except for function calls";

    public string LongDescription =>
        "This rule checks if an if statement contains operators, except for function calls, and raises a warning if it does.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var ifStatements = root.DescendantNodes()
            .OfType<IfStatementSyntax>()
            .ToArray();

        var notCool = ifStatements
            .Where(IfStatementContainsOperator)
            .ToArray();

        foreach (var ifStatement in notCool)
        {
            FileLinePositionSpan span = ifStatement.SyntaxTree.GetLineSpan(ifStatement.Span);
            int lineNumber = span.StartLinePosition.Line + 1;

            var message = new ValidationMessage(
                Severity.Warning,
                Id,
                Name,
                $"An if statement in the file '{filePath}':{lineNumber} contains operators, which is not allowed."
            );
            messages.Add(message);
        }

        return messages.ToArray();
    }

    private bool IfStatementContainsOperator(IfStatementSyntax ifStatement)
    {
        var nodes = ifStatement.Condition.DescendantNodesAndSelf();
        bool hasOperator = nodes.Any(n =>
            (n is BinaryExpressionSyntax binaryExpression && !(IsSimpleNullComparison(binaryExpression))) ||
            n is PostfixUnaryExpressionSyntax);

        return hasOperator;
    }

    private bool IsSimpleNullComparison(BinaryExpressionSyntax binaryExpression)
    {
        return (binaryExpression.Left is LiteralExpressionSyntax leftLiteral &&
                leftLiteral.IsKind(SyntaxKind.NullLiteralExpression)) ||
               (binaryExpression.Right is LiteralExpressionSyntax rightLiteral &&
                rightLiteral.IsKind(SyntaxKind.NullLiteralExpression));
    }
}