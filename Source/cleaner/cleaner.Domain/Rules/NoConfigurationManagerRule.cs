using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Rules;

public class NoConfigurationManagerRule : IRule
{
    public string Id => "NoConfigurationManager";
    public string Name => "No ConfigurationManager Rule";
    public string ShortDescription => "Check if ConfigurationManager or WebConfigurationManager is used in the code";
    public string LongDescription => "This rule checks if ConfigurationManager or WebConfigurationManager is used anywhere in the code. It will return an error if either is found.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        var tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetRoot();

        // Find all method invocations of ConfigurationManager and WebConfigurationManager
        var configurationManagerInvocations = root.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Where(m => (m.Name.Identifier.ValueText == "ConfigurationManager" || m.Name.Identifier.ValueText == "WebConfigurationManager") && m.Expression != null)
            .Select(m => m.Expression.ToString())
            .Distinct();

        // Check if any ConfigurationManager or WebConfigurationManager invocations were found
        if (configurationManagerInvocations.Any())
        {
            messages.Add(new ValidationMessage(
                Severity.Error,
                Id,
                Name,
                $"The file '{filePath}' contains references to ConfigurationManager or WebConfigurationManager. Please avoid using these APIs as they are considered legacy and have been deprecated in .NET Core."));
        }

        return messages.ToArray();
    }
}