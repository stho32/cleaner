using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.FileBasedRules.Rules;

public class NotImplementedExceptionRule : IRule
{
    public string Id => GetType().Name;
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

        var exceptionCount = CountNotImplementedExceptions(throwStatements);
        var exceptionsFound = exceptionCount > 0;

        if (exceptionsFound)
        {
            messages.Add(
                new ValidationMessage(
                    Id,
                    Name,
                    $"The file '{filePath}' contains {exceptionCount} occurrence(s) of 'NotImplementedException'. Please review and implement the required functionality."));
        }

        return messages.ToArray();
    }

    private static int CountNotImplementedExceptions(IEnumerable<ThrowStatementSyntax> throwStatements)
    {
        int exceptionCount = 0;
        foreach (var throwStatement in throwStatements)
        {
            if (IsNewNotImplementedExceptionStatement(throwStatement))
                exceptionCount++;
        }

        return exceptionCount;
    }

    private static bool IsNewNotImplementedExceptionStatement(ThrowStatementSyntax throwStatement)
    {
        return throwStatement.Expression is ObjectCreationExpressionSyntax objectCreation &&
               objectCreation.Type.ToString() == "NotImplementedException";
    }
}
