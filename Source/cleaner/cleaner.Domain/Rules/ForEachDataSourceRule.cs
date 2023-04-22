using cleaner.Domain.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class ForEachDataSourceRule : IRule
{
    public string Id => GetType().Name;
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
            if (DataSourceExpressionHasTooManyDots(forEachStatement))
            {
                int lineNumber = RuleHelper.GetLineNumber(forEachStatement);

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

    private bool DataSourceExpressionHasTooManyDots(ForEachStatementSyntax forEachStatement)
    {
        var dataSourceExpression = forEachStatement.Expression.ToString();
        var dotCount = dataSourceExpression.Count(c => c == '.');
        return dotCount > 2;
    }
}
