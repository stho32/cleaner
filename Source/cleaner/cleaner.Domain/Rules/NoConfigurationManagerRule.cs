namespace cleaner.Domain.Rules;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class NoConfigurationManagerRule : IRule
{
    public string Id => "NoConfigurationManagerRule";

    public string Name => "Configuration Management Detection Rule";

    public string ShortDescription => "Detects usage of .NET Configuration Management";

    public string LongDescription => "This rule checks for usage of the .NET Configuration Management, including both normal and web versions, and raises a warning if detected.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        
        var usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>();

        foreach (var usingDirective in usings)
        {
            if (usingDirective.Name.ToString() == "System.Configuration" || usingDirective.Name.ToString() == "System.Web.Configuration")
            {
                messages.Add(new ValidationMessage(Severity.Warning, Id, Name, $"Configuration Management detected in file '{filePath}' at line {GetLineNumber(usingDirective)}"));
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
