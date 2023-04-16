using System.Text.RegularExpressions;

namespace cleaner.Domain.Rules;

public class SingleDeclarationRule : IRule
{
    private readonly string[] _keywords = { "enum", "class", "interface", "record", "struct" };

    public string Id => "E002";
    public string Name => "Single Declaration Rule";
    public string ShortDescription => "Check if a file contains only one type declaration.";
    public string LongDescription => "This rule checks if a file contains only one of the specified keywords: enum, class, interface, record, or struct. It will return an error if more than one keyword is found in the file.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        int keywordCount = _keywords.Sum(keyword => Regex.Matches(fileContent, $@"\b{keyword}\b").Count);

        if (keywordCount > 1)
        {
            messages.Add(
                new ValidationMessage(
                Severity.Error,
                Id,
                Name,
                $"The file '{filePath}' contains more than one of the following keywords: {string.Join(", ", _keywords)}. Only one type declaration is allowed per file."));
        }

        return messages.ToArray();
    }
}