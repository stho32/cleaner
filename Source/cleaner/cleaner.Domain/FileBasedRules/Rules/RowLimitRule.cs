using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.FileBasedRules.Rules;

public class RowLimitRule : IRule
{
    private readonly int _maxRows;

    public RowLimitRule(int maxRows = 500)
    {
        _maxRows = maxRows;
    }

    public string Id => GetType().Name;
    public string Name => "Row Limit Rule";
    public string ShortDescription => $"Detects files with more than {_maxRows} rows";
    public string LongDescription => $"This rule checks if a file has more than {_maxRows} rows and raises a warning if it does.";

    public ValidationMessage[] Validate(string filePath, string fileContent, SyntaxTree tree, CompilationUnitSyntax root)
    {
        int rowCount = CountRows(fileContent);
        var hasMoreRowsThanAllowed = rowCount > _maxRows;

        if (hasMoreRowsThanAllowed)
        {
            var message = new ValidationMessage(
                Id,
                Name,
                $"The file '{filePath}' contains {rowCount} rows, which is more than the allowed limit of {_maxRows} rows."
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