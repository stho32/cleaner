namespace cleaner.Domain.Rules;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class SingleDeclarationRule : IRule
{
    public string Id => "E002";
    public string Name => "Single Declaration Rule";
    public string ShortDescription => "Check if a file contains only one type declaration.";
    public string LongDescription => "This rule checks if a file contains only one of the specified keywords: enum, class, interface, record, or struct. It will return an error if more than one keyword is found in the file.";

    public ValidationMessage?[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        int declarationCount = root.Members.OfType<BaseTypeDeclarationSyntax>().Count();

        if (declarationCount > 1)
        {
            messages.Add(
                new ValidationMessage(
                    Severity.Error,
                    Id,
                    Name,
                    $"The file '{filePath}' contains {declarationCount} type declarations. Only one type declaration is allowed per file."));
        }

        return messages.ToArray();
    }
}