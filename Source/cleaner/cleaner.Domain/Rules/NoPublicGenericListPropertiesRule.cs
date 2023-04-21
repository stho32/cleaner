using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class NoPublicGenericListPropertiesRule : IRule
{
    public string Id => "NoPublicGenericListPropertiesRule";
    public string Name => "No Public Generic List Properties Rule";
    public string ShortDescription => "Disallow public properties of type List<T>";
    public string LongDescription => "This rule checks if there are any public properties of type List<T> and raises a warning if any are found.";

    public ValidationMessage?[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var propertyDeclarations = root.DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .Where(pd => pd.Parent is ClassDeclarationSyntax || pd.Parent is NamespaceDeclarationSyntax)
            .ToArray();

        foreach (var propertyDeclaration in propertyDeclarations)
        {
            if (IsPublic(propertyDeclaration) && IsGenericListType(propertyDeclaration))
            {
                var message = new ValidationMessage(
                    Severity.Warning,
                    Id,
                    Name,
                    $"The file '{filePath}' contains a public property of type List<T>: '{propertyDeclaration.Identifier.Text}'. This is not allowed."
                );
                messages.Add(message);
            }
        }

        return messages.ToArray();
    }

    private bool IsPublic(PropertyDeclarationSyntax propertyDeclaration)
    {
        return propertyDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword));
    }

    private bool IsGenericListType(PropertyDeclarationSyntax propertyDeclaration)
    {
        return propertyDeclaration.Type is GenericNameSyntax genericType &&
               genericType.Identifier.Text == "List";
    }

}