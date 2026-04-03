using cleaner.Domain.FileBasedRules.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests;

public class StatisticsTests
{
    [Test]
    public void EmptyMessages_ZeroTotals()
    {
        var messages = Array.Empty<ValidationMessage>();
        var statistic = new Statistic(0, 0, 5, 2);

        var stats = new Statistics(messages, statistic);

        Assert.That(stats.TotalMessages, Is.EqualTo(0));
        Assert.That(stats.MessagesPerRule, Is.Empty);
    }

    [Test]
    public void MultipleMessages_GroupedByRule()
    {
        var messages = new[]
        {
            new ValidationMessage("Rule1", "Name1", "Error1"),
            new ValidationMessage("Rule1", "Name1", "Error2"),
            new ValidationMessage("Rule2", "Name2", "Error3"),
        };
        var statistic = new Statistic(2, 0, 10, 3);

        var stats = new Statistics(messages, statistic);

        Assert.That(stats.TotalMessages, Is.EqualTo(3));
        Assert.That(stats.MessagesPerRule["Rule1"], Is.EqualTo(2));
        Assert.That(stats.MessagesPerRule["Rule2"], Is.EqualTo(1));
    }

    [Test]
    public void PrintStatistics_DoesNotThrow()
    {
        var messages = new[] { new ValidationMessage("R1", "Name", "Err") };
        var statistic = new Statistic(1, 0, 5, 2);
        var stats = new Statistics(messages, statistic);

        Assert.DoesNotThrow(() => stats.PrintStatistics());
    }
}
