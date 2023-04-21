namespace cleaner.Domain.Rules;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class LinqExpressionLengthRule : IRule
{
    public string Id => "LinqExpressionLengthRule";
    public string Name => "LINQ Expression Length Rule";
    public string ShortDescription => "Detects LINQ expressions with more than 2 steps";
    public string LongDescription => "This rule checks if a LINQ expression contains more than 2 steps and raises a warning if it does.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var invocationExpressions = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        foreach (var invocationExpression in invocationExpressions)
        {
            var methodCalls = invocationExpression.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(IsALinqPredicate)
                .ToList();

            var linqExpressionIsTooLong = methodCalls.Count > 2;
            
            if (linqExpressionIsTooLong)
            {
                var message = new ValidationMessage(
                    Severity.Warning,
                    Id,
                    Name,
                    $"A LINQ expression in the file '{filePath}' contains {methodCalls.Count} steps, which is more than the allowed limit of 2."
                );
                messages.Add(message);
            }
        }

        return messages.ToArray();
    }

    private static bool IsALinqPredicate(IdentifierNameSyntax identifierNameSyntax)
    {
        var name = identifierNameSyntax.Identifier.Text;
        
        var possibleKeywords = new[]
        {
            "Where",
            "Select",
            "GroupBy",
            "OrderBy",
            "Join",
            "GroupJoin"
        };

        foreach (var possibleKeyword in possibleKeywords)
        {
            if (name.StartsWith(possibleKeyword))
                return true;
        }

        return false;
    }
}