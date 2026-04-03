using cleaner.Domain.FileBasedRules.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.FileBasedRules.Rules.NestedIfStatementsRuleValidation;

public class NestedIfStatementsRule : IRule
{
    private readonly int _maxDepth;

    public NestedIfStatementsRule(int maxDepth = 2)
    {
        _maxDepth = maxDepth;
    }

    public string Id => GetType().Name;

    public string Name => "Nested If Statements Rule";

    public string ShortDescription => "Detects methods with if statements nested more than 2 levels deep";

    public string LongDescription => "This rule checks if a method contains if statements nested more than 2 levels deep. If it does, a warning is raised.";

    public ValidationMessage[] Validate(string filePath, string fileContent, SyntaxTree tree, CompilationUnitSyntax root)
    {
        var messages = new List<ValidationMessage>();

        var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        foreach (var methodDeclaration in methodDeclarations)
        {
            var analyzer = new NestingLevelAnalyzer();
            int maxNestingLevel = analyzer.GetMaxNestingLevel(methodDeclaration.Body);
            bool tooMuchNesting = maxNestingLevel > _maxDepth;

            if (tooMuchNesting)
            {
                messages.Add(new ValidationMessage(Id, Name, $"Method '{methodDeclaration.Identifier.Text}' in file '{filePath}' at line {RuleHelper.GetLineNumber(methodDeclaration)} has if statements nested more than {_maxDepth} levels deep."));
            }
        }

        return messages.ToArray();
    }
}