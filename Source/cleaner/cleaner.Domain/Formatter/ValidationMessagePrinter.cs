using System.Drawing;
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"({message.RuleId}): {message.ErrorMessage}");
        }

        Console.ResetColor();
    }
}
