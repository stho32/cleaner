using cleaner.Domain.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class RepositoryConstructorRule : IRule
{
    public string Id => GetType().Name;
    public string Name => "Repository Constructor Rule";

    public string ShortDescription => "Detects 'Repository' classes without a constructor with at least one parameter of type 'IDatabaseAccessor'";

    public string LongDescription => "This rule checks if a class ending with 'Repository' has a constructor with at least one parameter of type 'IDatabaseAccessor'. If it doesn't, a warning is raised.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classDeclarations)
        {
            if (HasMatchingClassName(classDeclaration))
            {
                bool hasRequiredConstructor = false;

                foreach (var constructor in classDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>())
                {
                    if (HasParameterOfTypeIDatabaseAccessor(constructor))
                    {
                        hasRequiredConstructor = true;
                        break;
                    }
                }

                if (!hasRequiredConstructor)
                {
                    messages.Add(new ValidationMessage(Id, Name, $"Class '{classDeclaration.Identifier.Text}' in file '{filePath}' at line {RuleHelper.GetLineNumber(classDeclaration)} should have a constructor with at least one parameter of type 'IDatabaseAccessor'."));
                }
            }
        }

        return messages.ToArray();
    }

    private static bool HasParameterOfTypeIDatabaseAccessor(ConstructorDeclarationSyntax constructor)
    {
        var parameters = constructor.ParameterList.Parameters;
        var requiredParameterType = "IDatabaseAccessor";
        var hasRequiredParameter = parameters.Any(param => param.Type?.ToString() == requiredParameterType);
        return hasRequiredParameter;
    }

    private static bool HasMatchingClassName(ClassDeclarationSyntax classDeclaration)
    {
        var className = classDeclaration.Identifier.Text;
        var classNameEndsWithRepository = className.EndsWith("Repository", StringComparison.OrdinalIgnoreCase);
        return classNameEndsWithRepository;
    }
}