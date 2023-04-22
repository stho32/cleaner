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

    public string Id => GetType().Name;
    public string Name => "Allowed Usings Rule";
    public string ShortDescription => "Verify that only allowed usings are used";
    public string LongDescription => "This rule checks if only allowed usings are used in a file and raises a warning if any disallowed usings are found.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);

        var root = tree.GetCompilationUnitRoot();
        string rootNamespace = ExtractRootNamespace(root);

        var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();

        foreach (var usingDirective in usingDirectives)
        {
            string usingNamespace = GetUsingWithoutOptionalGlobalMarker(usingDirective);

            var isInvalidUsing = !_allowedUsings.Contains(usingNamespace) && 
                    !IsSubNamespaceOfSameRootNamespace(usingNamespace, rootNamespace);
            
            if (isInvalidUsing)
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

    private string GetUsingWithoutOptionalGlobalMarker(UsingDirectiveSyntax usingDirective)
    {
        var usingNamespace = usingDirective.Name.ToString();
        
        if (usingNamespace.StartsWith("global::"))
        {
            usingNamespace = usingNamespace.Replace("global::", "");
        }

        return usingNamespace;
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
    
    public static HashSet<string> GetDefaultAllowedUsings()
    {
        return new HashSet<string>
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "Newtonsoft.Json",
            "Newtonsoft.Json.Linq",
            "System.Data",
            "System.Data.SqlClient",
            "System.Text",
            "System.ComponentModel",
            "System.Web",
            "System.Web.Mvc",
            "System.Web.Routing",
            "Newtonsoft.Json.Serialization",
            "WebGrease.Css.Extensions",
            "System.IO",
            "Microsoft.CodeAnalysis",
            "Microsoft.CodeAnalysis.CSharp",
            "Microsoft.CodeAnalysis.CSharp.Syntax",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
            "System.Runtime.CompilerServices",
            "System.Text.RegularExpressions",
            "System.Reflection"
        };
    }
}