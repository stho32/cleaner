using System.Text.RegularExpressions;

namespace cleaner.Domain.Rules;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class MethodLengthRule : IRule
{
    public string Id => "MethodLengthRule";
    public string Name => "Method Length Rule";
    public string ShortDescription => "Detects methods with more than 10 semicolons in their body";
    public string LongDescription => "This rule checks if a method contains more than 10 semicolons in its body and raises a warning if it does.";

    public ValidationMessage?[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var methods = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>();

        foreach (var method in methods)
        {
            var semicolonCount = method.DescendantTokens()
                .Count(token => token.IsKind(SyntaxKind.SemicolonToken));

            if (semicolonCount > 10)
            {
                var message = new ValidationMessage(
                    Severity.Warning,
                    Id,
                    Name,
                    $"A method '{method.Identifier.Text}' in the file '{filePath}' contains {semicolonCount} semicolons, which is more than the allowed limit of 10."
                );
                messages.Add(message);
            }
        }

        return messages.ToArray();
    }
}
