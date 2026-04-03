using cleaner.Domain.CommandLineArguments;
using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.FileBasedRules.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests;

public class CompositeQualityScannerTests
{
    private class FakeScanner : IQualityScanner
    {
        private readonly ValidationMessage[] _messages;
        public FakeScanner(params ValidationMessage[] messages) => _messages = messages;
        public ValidationMessage[] PerformQualityScan(CommandLineOptions commandLineOptions, IDirectoryWalker directoryWalker)
            => _messages;
    }

    [Test]
    public void NoScanners_ReturnsEmpty()
    {
        var composite = new CompositeQualityScanner();
        var result = composite.PerformQualityScan(null!, null!);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void SingleScanner_ReturnsScannerMessages()
    {
        var msg = new ValidationMessage("R1", "Rule", "Error");
        var composite = new CompositeQualityScanner(new FakeScanner(msg));

        var result = composite.PerformQualityScan(null!, null!);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].RuleId, Is.EqualTo("R1"));
    }

    [Test]
    public void MultipleScanners_CombinesMessages()
    {
        var msg1 = new ValidationMessage("R1", "Rule1", "Error1");
        var msg2 = new ValidationMessage("R2", "Rule2", "Error2");
        var msg3 = new ValidationMessage("R3", "Rule3", "Error3");

        var composite = new CompositeQualityScanner(
            new FakeScanner(msg1, msg2),
            new FakeScanner(msg3)
        );

        var result = composite.PerformQualityScan(null!, null!);

        Assert.That(result, Has.Length.EqualTo(3));
    }
}
