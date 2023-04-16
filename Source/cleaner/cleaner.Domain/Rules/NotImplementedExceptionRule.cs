using System.Text.RegularExpressions;

namespace cleaner.Domain.Rules;

public class NotImplementedExceptionRule : IRule
{
    public string Id => "E003";
    public string Name => "Not Implemented Exception Rule";
    public string ShortDescription => "Check if a file contains NotImplementedException.";
    public string LongDescription => "This rule checks if a file contains the keyword 'NotImplementedException'. It will return a warning if the keyword is found in the file.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        int exceptionCount = Regex.Matches(fileContent, @"\bNotImplementedException\b").Count;

        if (exceptionCount > 0)
        {
            messages.Add(
                new ValidationMessage(
                    Severity.Warning,
                    Id,
                    Name,
                    $"The file '{filePath}' contains {exceptionCount} occurrence(s) of 'NotImplementedException'. Please review and implement the required functionality."));
        }

        return messages.ToArray();
    }
}