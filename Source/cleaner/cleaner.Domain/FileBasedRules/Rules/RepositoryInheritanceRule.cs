using cleaner.Domain.FileBasedRules.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.FileBasedRules.Rules;

public class RepositoryInheritanceRule : IRule
{
    public string Id => GetType().Name;

    public string Name => "Repository Inheritance Rule";

    public string ShortDescription => "Detects 'Repository' classes derived from other classes";

    public string LongDescription => "This rule checks if a class ending with 'Repository' is derived from another class. If it is, a warning is raised.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classDeclarations)
        {
            var className = classDeclaration.Identifier.Text;
            var isRepositoryClass = className.EndsWith("Repository", StringComparison.OrdinalIgnoreCase);

            if (isRepositoryClass)
            {
                if (InheritsFromClass(classDeclaration))
                {
                    messages.Add(new ValidationMessage(Id, Name, $"Class '{className}' in file '{filePath}' at line {RuleHelper.GetLineNumber(classDeclaration)} should not inherit from another class. Inheriting from interfaces is allowed."));
                }
            }
        }

        return messages.ToArray();
    }

    private static bool InheritsFromClass(ClassDeclarationSyntax classDeclaration)
    {
        if (classDeclaration.BaseList == null)
            return false;

        return classDeclaration.BaseList.Types.Any(baseType =>
        {
            var symbol = baseType.Type switch
            {
                IdentifierNameSyntax ins => ins.Identifier.Text,
                QualifiedNameSyntax qns => qns.Right.Identifier.Text,
                _ => null
            };

            return symbol != null && !symbol.StartsWith("I");
        });
    }
}