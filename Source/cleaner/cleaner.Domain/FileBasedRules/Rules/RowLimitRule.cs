namespace cleaner.Domain.FileBasedRules.Rules;

public class RowLimitRule : IRule
{
    public string Id => GetType().Name;
    public string Name => "Row Limit Rule";
    public string ShortDescription => "Detects files with more than 500 rows";
    public string LongDescription => "This rule checks if a file has more than 500 rows and raises a warning if it does.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        int rowCount = CountRows(fileContent);
        var hasMoreRowsThanAllowed = rowCount > 500;

        if (hasMoreRowsThanAllowed)
        {
            var message = new ValidationMessage(
                Id,
                Name,
                $"The file '{filePath}' contains {rowCount} rows, which is more than the allowed limit of 500 rows."
            );
            return new[] { message };
        }
        return Array.Empty<ValidationMessage>();
    }

    private int CountRows(string content)
    {
        return content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length;
    }
}