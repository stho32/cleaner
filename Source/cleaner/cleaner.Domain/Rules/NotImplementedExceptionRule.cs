namespace cleaner.Domain.Rules;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class NotImplementedExceptionRule : IRule
{
    public string Id => "E003";
    public string Name => "Not Implemented Exception Rule";
    public string ShortDescription => "Check if a file contains NotImplementedException.";
    public string LongDescription => "This rule checks if a file contains the keyword 'NotImplementedException'. It will return a warning if the keyword is found in the file.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var throwStatements = root.DescendantNodes()
            .OfType<ThrowStatementSyntax>();

        int exceptionCount = 0;
        foreach (var throwStatement in throwStatements)
        {
            if (throwStatement.Expression is ObjectCreationExpressionSyntax objectCreation &&
                objectCreation.Type.ToString() == "NotImplementedException")
            {
                exceptionCount++;
            }
        }

        if (exceptionCount > 0)
        {
            messages.Add(
                new ValidationMessage(
                    Severity.Warning,
                    Id,
                    Name,
                    $"The file '{filePath}' contains {exceptionCount} occurrence(s) of 'NotImplementedException'. Please review and implement the required functionality."));
        }

        return messages.ToArray();
    }
}
