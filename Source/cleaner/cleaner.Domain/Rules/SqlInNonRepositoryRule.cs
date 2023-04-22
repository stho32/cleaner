using System.Text.RegularExpressions;
using cleaner.Domain.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

// cleaner: ignore SqlInNonRepositoryRule
public class SqlInNonRepositoryRule : IRule
{
    public string Id => GetType().Name;

    public string Name => "SQL in Non-Repository Class Rule";

    public string ShortDescription => "Detects SQL strings in classes not ending with 'Repository'";

    public string LongDescription => "This rule checks for SQL strings within classes, and if found, ensures the class name ends with 'Repository'. If not, a warning is raised.";

    private static readonly Regex SqlRegex = new Regex(@"\b(?:SELECT|TOP|INSERT|UPDATE|DELETE|FROM|WHERE|EXEC|ORDER BY)\b", RegexOptions.IgnoreCase);
    private const int SqlKeywordThreshold = 3;

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classDeclarations)
        {
            var className = classDeclaration.Identifier.Text;
            if (!IsRepository(className))
            {
                var stringLiterals = classDeclaration.DescendantNodes().OfType<LiteralExpressionSyntax>()
                    .Where(node => node.IsKind(SyntaxKind.StringLiteralExpression))
                    .ToArray();

                foreach (var stringLiteral in stringLiterals)
                {
                    var matches = SqlRegex.Matches(stringLiteral.Token.ValueText);

                    var isASubstentialAmountOfSqlPresent = matches.Count >= SqlKeywordThreshold;
                    if (isASubstentialAmountOfSqlPresent)
                    {
                        messages.Add(new ValidationMessage(Id, Name, $"SQL detected in non-Repository class '{className}' in file '{filePath}' at line {RuleHelper.GetLineNumber(stringLiteral)}"));
                    }
                }
            }
        }

        return messages.ToArray();
    }

    private static bool IsRepository(string className)
    {
        return className.EndsWith("Repository", StringComparison.OrdinalIgnoreCase);
    }
}
