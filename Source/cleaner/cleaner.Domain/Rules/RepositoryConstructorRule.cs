using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class RepositoryConstructorRule : IRule
{
    public string Id => GetType().Name;

    public string Name => "Repository Constructor Rule";

    public string ShortDescription => "Detects 'Repository' classes without a constructor with at least one parameter of type 'IDatabaseAccessor'";

    public string LongDescription => "This rule checks if a class ending with 'Repository' has a constructor with at least one parameter of type 'IDatabaseAccessor'. If it doesn't, a warning is raised.";

    public ValidationMessage?[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classDeclarations)
        {
            if (classDeclaration.Identifier.Text.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
            {
                bool hasRequiredConstructor = false;

                foreach (var constructor in classDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>())
                {
                    if (constructor.ParameterList.Parameters.Any(param => param.Type?.ToString() == "IDatabaseAccessor"))
                    {
                        hasRequiredConstructor = true;
                        break;
                    }
                }

                if (!hasRequiredConstructor)
                {
                    messages.Add(new ValidationMessage(Severity.Warning, Id, Name, $"Class '{classDeclaration.Identifier.Text}' in file '{filePath}' at line {GetLineNumber(classDeclaration)} should have a constructor with at least one parameter of type 'IDatabaseAccessor'."));
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