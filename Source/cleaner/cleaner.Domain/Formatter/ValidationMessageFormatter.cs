using cleaner.Domain.Rules;

namespace cleaner.Domain.Formatter;

public class ValidationMessagePrinter
{
    public void Print(ValidationMessage[] messages)
    {
        foreach (var message in messages)
        {
            Console.ForegroundColor = GetSeverityColor(message.Severity);
            Console.WriteLine($"{message.Severity}: {message.RuleName} ({message.RuleId}): {message.ErrorMessage}");
        }

        Console.ResetColor();

        // Print the count of messages
        Console.WriteLine($"\nTotal messages: {messages.Length}");
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
