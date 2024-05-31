using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.Helpers;

namespace cleaner.Domain.Formatter;

public class ValidationMessagePrinter
{
    private readonly int _maxRuleIdWidth;

    public ValidationMessagePrinter(int maxRuleIdWidth)
    {
        _maxRuleIdWidth = maxRuleIdWidth;
    }

    public void Print(ValidationMessage[]? messages)
    {
        if (CollectionHelpers.IsNullOrEmpty(messages))
            return;

        string format = $"{{0, -{_maxRuleIdWidth}}}   {{1}}";
        Console.ForegroundColor = ConsoleColor.Yellow;

        foreach (var message in messages!)
        {
            Console.WriteLine(format, $"({message.RuleId}):", message.ErrorMessage);
        }

        Console.ResetColor();
    }
}