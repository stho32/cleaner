using cleaner.Domain.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules.NestedIfStatementsRuleValidation;

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
            var analyzer = new NestingLevelAnalyzer();
            int maxNestingLevel = analyzer.GetMaxNestingLevel(methodDeclaration.Body);
            bool tooMuchNesting = maxNestingLevel > 2;  

            if (tooMuchNesting)
            {
                messages.Add(new ValidationMessage(Severity.Warning, Id, Name, $"Method '{methodDeclaration.Identifier.Text}' in file '{filePath}' at line {RuleHelper.GetLineNumber(methodDeclaration)} has if statements nested more than 2 levels deep."));
            }
        }

        return messages.ToArray();
    }
}