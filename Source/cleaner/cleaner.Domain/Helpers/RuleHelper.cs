using Microsoft.CodeAnalysis;

namespace cleaner.Domain.Helpers;

public static class RuleHelper
{
    public static int GetLineNumber(SyntaxNode? node)
    {
        if (node == null)
            return -1;

        var lineSpan = node.SyntaxTree.GetLineSpan(node.Span);
        return lineSpan.StartLinePosition.Line + 1;
    }
}