using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules.NestedIfStatementsRuleValidation;

public class NestingLevelAnalyzer
{
    public int GetMaxNestingLevel(SyntaxNode? node)
    {
        return AnalyzeNode(node, 0);
    }

    private int AnalyzeNode(SyntaxNode? node, int currentNestingLevel)
    {
        if (node == null)
            return currentNestingLevel;

        int maxNestingLevel = currentNestingLevel;

        if (IsAnIfStatement(node))
        {
            currentNestingLevel++;
            maxNestingLevel = currentNestingLevel;
        }

        foreach (var child in node.ChildNodes())
        {
            maxNestingLevel = Math.Max(maxNestingLevel, AnalyzeNode(child, currentNestingLevel));
        }

        return maxNestingLevel;
    }

    private static bool IsAnIfStatement(SyntaxNode node)
    {
        return node is IfStatementSyntax;
    }
}