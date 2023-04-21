using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class NoPublicFieldsRule : IRule
{
    public string Id => "E006";
    public string Name => "No Public Fields Rule";
    public string ShortDescription => "Checks if any public fields are declared in a file.";
    public string LongDescription => "This rule checks if any public fields are declared in a file. It will return a warning if a public field is found.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var fieldDeclarations = root.DescendantNodes()
            .OfType<FieldDeclarationSyntax>()
            .Where(fd => IsPublic(fd));

        foreach (var fieldDeclaration in fieldDeclarations)
        {
            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                var message = new ValidationMessage(
                    Severity.Warning,
                    Id,
                    Name,
                    $"The file '{filePath}' contains a public field: '{variable.Identifier.Text}'. This is not allowed."
                );
                messages.Add(message);
            }
        }

        return messages.ToArray();
    }

    private bool IsPublic(FieldDeclarationSyntax fieldDeclaration)
    {
        return fieldDeclaration.Modifiers.Any(modifier =>
            modifier.IsKind(SyntaxKind.PublicKeyword) &&
            !modifier.IsKind(SyntaxKind.StaticKeyword));
    }
}