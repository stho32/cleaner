using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.Formatter;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Formatter;

public class ValidationMessagePrinterTests
{
    [Test]
    public void Print_NullMessages_DoesNotThrow()
    {
        var printer = new ValidationMessagePrinter();
        Assert.DoesNotThrow(() => printer.Print(null));
    }

    [Test]
    public void Print_EmptyMessages_DoesNotThrow()
    {
        var printer = new ValidationMessagePrinter();
        Assert.DoesNotThrow(() => printer.Print(Array.Empty<ValidationMessage>()));
    }

    [Test]
    public void Print_WithMessages_WritesToConsole()
    {
        var printer = new ValidationMessagePrinter();
        var messages = new[]
        {
            new ValidationMessage("R1", "Rule1", "Error message 1"),
            new ValidationMessage("R2", "Rule2", "Error message 2"),
        };

        var output = new StringWriter();
        Console.SetOut(output);

        printer.Print(messages);

        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });

        var text = output.ToString();
        Assert.That(text, Does.Contain("R1"));
        Assert.That(text, Does.Contain("Error message 1"));
        Assert.That(text, Does.Contain("R2"));
    }
}
