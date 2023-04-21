using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class SqlInNonRepositoryRule : IRule
{
    public string Id => GetType().Name;

    public string Name => "SQL in Non-Repository Class Rule";

    public string ShortDescription => "Detects SQL strings in classes not ending with 'Repository'";

    public string LongDescription => "This rule checks for SQL strings within classes, and if found, ensures the class name ends with 'Repository'. If not, a warning is raised.";

    private static readonly Regex SqlRegex = new Regex(@"\b(?:SELECT|INSERT|UPDATE|DELETE)\b", RegexOptions.IgnoreCase);

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classDeclarations)
        {
            if (!classDeclaration.Identifier.Text.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
            {
                var stringLiterals = classDeclaration.DescendantNodes().OfType<LiteralExpressionSyntax>()
                    .Where(node => node.IsKind(SyntaxKind.StringLiteralExpression));

                foreach (var stringLiteral in stringLiterals)
                {
                    if (SqlRegex.IsMatch(stringLiteral.Token.ValueText))
                    {
                        messages.Add(new ValidationMessage(Severity.Warning, Id, Name, $"SQL detected in non-Repository class '{classDeclaration.Identifier.Text}' in file '{filePath}' at line {GetLineNumber(stringLiteral)}"));
                    }
                }
            }
        }

        return messages.ToArray();
    }

    private int GetLineNumber(SyntaxNode node)
    {
        var lineSpan = node.SyntaxTree.GetLineSpan(node.Span);
        return lineSpan.StartLinePosition.Line + 1;
    }
}