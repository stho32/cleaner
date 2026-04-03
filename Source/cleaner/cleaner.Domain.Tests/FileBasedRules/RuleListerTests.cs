using cleaner.Domain.FileBasedRules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules;

public class RuleListerTests
{
    [Test]
    public void ListAllRules_DoesNotThrow()
    {
        var lister = new RuleLister();

        var output = new StringWriter();
        Console.SetOut(output);

        Assert.DoesNotThrow(() => lister.ListAllRules());

        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });

        var text = output.ToString();
        Assert.That(text, Does.Contain("List of existing rules:"));
        Assert.That(text, Does.Contain("Method Length Rule"));
    }
}
