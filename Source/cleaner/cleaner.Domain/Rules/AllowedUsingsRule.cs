using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class AllowedUsingsRule : IRule
{
    private readonly HashSet<string> _allowedUsings;

    public AllowedUsingsRule(HashSet<string> allowedUsings)
    {
        _allowedUsings = allowedUsings;
    }

    public string Id => "AllowedUsingsRule";
    public string Name => "Allowed Usings Rule";
    public string ShortDescription => "Verify that only allowed usings are used";
    public string LongDescription => "This rule checks if only allowed usings are used in a file and raises a warning if any disallowed usings are found.";

    public ValidationMessage?[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();

        foreach (var usingDirective in usingDirectives)
        {
            string usingNamespace = usingDirective.Name.ToString();

            if (!_allowedUsings.Contains(usingNamespace.Replace("global::", "")) && !IsSubNamespaceOfRootNamespace(usingNamespace, root))
            {
                var message = new ValidationMessage(
                    Severity.Warning,
                    Id,
                    Name,
                    $"The file '{filePath}' contains a disallowed using: '{usingNamespace}'."
                );
                messages.Add(message);
            }
        }

        return messages.ToArray();
    }

    private bool IsSubNamespaceOfRootNamespace(string usingNamespace, CompilationUnitSyntax root)
    {
        var namespaceDeclaration = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

        if (namespaceDeclaration != null)
        {
            string rootNamespace = namespaceDeclaration.Name.ToString();
            string firstPartOnly = rootNamespace.Split(".").First();
            return usingNamespace.StartsWith(firstPartOnly);
        }

        return false;
    }
}