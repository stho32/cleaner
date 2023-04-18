namespace cleaner.Domain.Rules;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class IfStatementOperatorRule : IRule
{
    public string Id => "IfStatementOperatorRule";
    public string Name => "If Statement Operator Rule";
    public string ShortDescription => "Detects if statements containing operators, except for function calls";
    public string LongDescription => "This rule checks if an if statement contains operators, except for function calls, and raises a warning if it does.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var ifStatements = root.DescendantNodes()
            .OfType<IfStatementSyntax>()
            .Where(IfStatementContainsOperator);

        foreach (var ifStatement in ifStatements)
        {
            var message = new ValidationMessage(
                Severity.Warning,
                Id,
                Name,
                $"An if statement in the file '{filePath}' contains operators, which is not allowed."
            );
            messages.Add(message);
        }

        return messages.ToArray();
    }

    private bool IfStatementContainsOperator(IfStatementSyntax ifStatement)
    {
        var nodes = ifStatement.Condition.DescendantNodesAndSelf();
        bool hasOperator = nodes.Any(n =>
            n is BinaryExpressionSyntax ||
            n is PrefixUnaryExpressionSyntax ||
            n is PostfixUnaryExpressionSyntax);

        bool hasInvocation = nodes.Any(n => n is InvocationExpressionSyntax);

        return hasOperator && !hasInvocation;
    }
}

