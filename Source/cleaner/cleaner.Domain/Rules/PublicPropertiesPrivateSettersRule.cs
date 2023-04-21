using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class PublicPropertiesPrivateSettersRule : IRule
{
    public string Id => "PublicPropertiesPrivateSettersRule";
    public string Name => "Public Properties Private Setters Rule";
    public string ShortDescription => "Public properties should not have public setters";
    public string LongDescription => "This rule checks if public properties have public setters and raises a warning if they do.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var propertyDeclarations = root.DescendantNodes().OfType<PropertyDeclarationSyntax>();

        foreach (var propertyDeclaration in propertyDeclarations)
        {
            if (IsPublicProperty(propertyDeclaration))
            {
                var setter = propertyDeclaration?.AccessorList?.Accessors.FirstOrDefault(a => a.Keyword.ValueText == "set");

                if (IsPublicSetter(setter))
                {
                    var message = new ValidationMessage(
                        Severity.Warning,
                        Id,
                        Name,
                        $"The property '{propertyDeclaration?.Identifier.Text}' in the file '{filePath}' has a public setter. Consider using a private setter."
                    );
                    messages.Add(message);
                }
            }
        }

        return messages.ToArray();
    }

    private static bool IsPublicSetter(AccessorDeclarationSyntax? setter)
    {
        return setter != null && setter.Modifiers.Any(modifier => modifier.ValueText == "public");
    }

    private static bool IsPublicProperty(PropertyDeclarationSyntax propertyDeclaration)
    {
        return propertyDeclaration.Modifiers.Any(modifier => modifier.ValueText == "public");
    }
}