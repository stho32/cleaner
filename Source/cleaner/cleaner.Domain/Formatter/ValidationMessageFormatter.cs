using cleaner.Domain.Helpers;
using cleaner.Domain.Rules;

namespace cleaner.Domain.Formatter;

public class ValidationMessagePrinter
{
    public void Print(ValidationMessage[]? messages)
    {
        if (CollectionHelpers.IsNullOrEmpty(messages))
            return;
        
        foreach (var message in messages!)
        {
            Console.ForegroundColor = GetSeverityColor(message.Severity);
            Console.WriteLine($"{message.Severity}: {message.RuleName} ({message.RuleId}): {message.ErrorMessage}");
        }

        Console.ResetColor();
    }

    private ConsoleColor GetSeverityColor(Severity severity)
    {
        switch (severity)
        {
            case Severity.Error:
                return ConsoleColor.Red;
            case Severity.Warning:
                return ConsoleColor.Yellow;
            default:
                return ConsoleColor.White;
        }
    }
}
