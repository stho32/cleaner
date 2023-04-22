using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace cleaner.Domain.IgnoreComments;

public static class CleanerCommentParser
{
    private static readonly Regex CleanerIgnorePattern = new Regex(@"\/\/\s*cleaner:\s*ignore\s+(\w+)", RegexOptions.IgnoreCase);

    public static HashSet<string> GetIgnoredRuleIds(string fileContent)
    {
        var ignoredRuleIds = new HashSet<string>();
        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        var singleLineComments = root.DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia));

        foreach (var commentTrivia in singleLineComments)
        {
            var match = CleanerIgnorePattern.Match(commentTrivia.ToString());
            if (match.Success)
            {
                var ruleId = match.Groups[1].Value;
                ignoredRuleIds.Add(ruleId);
            }
        }

        return ignoredRuleIds;
    }
}