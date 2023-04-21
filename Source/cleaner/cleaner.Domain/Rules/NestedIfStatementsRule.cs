using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class NestedIfStatementsRule : IRule
{
    public string Id => GetType().Name;

    public string Name => "Nested If Statements Rule";

    public string ShortDescription => "Detects methods with if statements nested more than 2 levels deep";

    public string LongDescription => "This rule checks if a method contains if statements nested more than 2 levels deep. If it does, a warning is raised.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        foreach (var methodDeclaration in methodDeclarations)
        {
            int maxNestingLevel = 0;
            AnalyzeNode(methodDeclaration?.Body, ref maxNestingLevel, 0);
            var nestingIsTooDeep = maxNestingLevel > 2;
            
            if (nestingIsTooDeep)
            {
                messages.Add(new ValidationMessage(Severity.Warning, Id, Name, $"Method '{methodDeclaration?.Identifier.Text}' in file '{filePath}' at line {GetLineNumber(methodDeclaration)} has if statements nested more than 2 levels deep."));
            }
        }

        return messages.ToArray();
    }

    private void AnalyzeNode(SyntaxNode? node, ref int maxNestingLevel, int currentNestingLevel)
    {
        if (node == null)
            return;
        
        if (IsAnIfStatement(node))
        {
            currentNestingLevel++;
            maxNestingLevel = Math.Max(maxNestingLevel, currentNestingLevel);
        }

        foreach (var child in node.ChildNodes())
        {
            AnalyzeNode(child, ref maxNestingLevel, currentNestingLevel);
        }
    }

    private static bool IsAnIfStatement(SyntaxNode node)
    {
        return node is IfStatementSyntax;
    }

    private int GetLineNumber(SyntaxNode? node)
    {
        if (node == null)
            return -1;
        
        var lineSpan = node.SyntaxTree.GetLineSpan(node.Span);
        return lineSpan.StartLinePosition.Line + 1;
    }
}

