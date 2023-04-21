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

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
        string rootNamespace = ExtractRootNamespace(root);

        foreach (var usingDirective in usingDirectives)
        {
            string usingNamespace = usingDirective.Name.ToString();
            if (usingNamespace.StartsWith("global::"))
            {
                usingNamespace = usingNamespace.Replace("global::", "");
            }

            if (!_allowedUsings.Contains(usingNamespace) && 
                !IsSubNamespaceOfSameRootNamespace(usingNamespace, rootNamespace))
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

    private string ExtractRootNamespace(CompilationUnitSyntax root)
    {
        var namespaceDeclaration = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        var fileScopedNamespaceDeclaration = root.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();

        if (namespaceDeclaration != null)
        {
            string fullNamespace = namespaceDeclaration.Name.ToString();
            string[] namespaceParts = fullNamespace.Split('.');
            return namespaceParts.Length > 0 ? namespaceParts[0] : string.Empty;
        }
        else if (fileScopedNamespaceDeclaration != null)
        {
            string fullNamespace = fileScopedNamespaceDeclaration.Name.ToString();
            string[] namespaceParts = fullNamespace.Split('.');
            return namespaceParts.Length > 0 ? namespaceParts[0] : string.Empty;
        }

        return string.Empty;
    }


    private bool IsSubNamespaceOfSameRootNamespace(string usingNamespace, string rootNamespace)
    {
        if (!string.IsNullOrEmpty(rootNamespace))
        {
            return usingNamespace.StartsWith(rootNamespace);
        }

        return false;
    }

    // private bool IsSubNamespaceOfRootNamespace(string usingNamespace, CompilationUnitSyntax root)
    // {
    //     var namespaceDeclaration = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
    //
    //     if (namespaceDeclaration != null)
    //     {
    //         string rootNamespace = namespaceDeclaration.Name.ToString();
    //         string firstPartOnly = rootNamespace.Split(".").First();
    //         return usingNamespace.StartsWith(firstPartOnly);
    //     }
    //
    //     return false;
    // }
}