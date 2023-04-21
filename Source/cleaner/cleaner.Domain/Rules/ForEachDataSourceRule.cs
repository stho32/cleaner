using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class ForEachDataSourceRule : IRule
{
    public string Id => "ForEachDataSourceRule";
    public string Name => "For Each Data Source Rule";

    public string ShortDescription =>
        "Detects foreach statements with data source expressions containing more than two dots";

    public string LongDescription =>
        "This rule checks if a foreach statement's data source expression contains more than two dots and raises a warning if it does.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var forEachStatements = root.DescendantNodes()
            .OfType<ForEachStatementSyntax>();

        foreach (var forEachStatement in forEachStatements)
        {
            var dataSourceExpression = forEachStatement.Expression.ToString();

            var dotCount = dataSourceExpression.Count(c => c == '.');
            var tooManyDots = dotCount > 2;
            
            if (tooManyDots)
            {
                FileLinePositionSpan span = forEachStatement.SyntaxTree.GetLineSpan(forEachStatement.Span);
                int lineNumber = span.StartLinePosition.Line + 1;

                var message = new ValidationMessage(
                    Severity.Warning,
                    Id,
                    Name,
                    $"A foreach statement in the file '{filePath}':{lineNumber} contains a data source expression with more than two dots, which is not allowed."
                );
                messages.Add(message);
            }
        }

        return messages.ToArray();
    }
}