using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class NoOutAndRefParametersRule : IRule
{
    public string Id => GetType().Name;
    public string Name => "No Out and Ref Parameters Rule";
    public string ShortDescription => "Checks if a method or indexer has out or ref parameters.";
    public string LongDescription => "This rule checks if a method or indexer has any out or ref parameters, and raises a warning if it does.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        var methodDeclarations = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(method => !method.Modifiers.Any(SyntaxKind.OverrideKeyword));

        var indexerDeclarations = root.DescendantNodes()
            .OfType<IndexerDeclarationSyntax>();

        foreach (var methodDeclaration in methodDeclarations)
        {
            foreach (var parameter in methodDeclaration.ParameterList.Parameters)
            {
                if (HasParameterRefOrOutModifier(parameter))
                {
                    messages.Add(new ValidationMessage(
                        Id,
                        Name,
                        $"Method '{methodDeclaration.Identifier.Text}' in file '{filePath}' has {parameter.Modifiers} parameter '{parameter.Identifier.Text}'. Please avoid using out or ref parameters."));
                }
            }
        }

        foreach (var indexerDeclaration in indexerDeclarations)
        {
            foreach (var parameter in indexerDeclaration.ParameterList.Parameters)
            {
                if (HasParameterRefOrOutModifier(parameter))
                {
                    messages.Add(new ValidationMessage(
                        Id,
                        Name,
                        $"Indexer in file '{filePath}' has {parameter.Modifiers} parameter '{parameter.Identifier.Text}'. Please avoid using out or ref parameters."));
                }
            }
        }

        return messages.ToArray();
    }

    private static bool HasParameterRefOrOutModifier(ParameterSyntax parameter)
    {
        return parameter.Modifiers.Any(SyntaxKind.RefKeyword) || parameter.Modifiers.Any(SyntaxKind.OutKeyword);
    }
}