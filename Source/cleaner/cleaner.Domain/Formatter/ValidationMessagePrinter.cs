using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.Helpers;

namespace cleaner.Domain.Formatter;

public class ValidationMessagePrinter
{
    public void Print(ValidationMessage[]? messages)
    {
        if (CollectionHelpers.IsNullOrEmpty(messages))
            return;

        Console.ForegroundColor = ConsoleColor.Yellow;

        foreach (var message in messages!)
        {
            Console.WriteLine($"Rule ID: {message.RuleId}");
            Console.WriteLine($"Error Message: {message.ErrorMessage}");
            Console.WriteLine("--------------------------------------------------");
        }

        Console.WriteLine("");
        Console.ResetColor();
    }
}