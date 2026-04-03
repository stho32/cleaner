using cleaner.Domain.Configuration;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Configuration;

public class CleanerConfigTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var config = new CleanerConfig();

        Assert.That(config.MethodLengthMaxSemicolons, Is.EqualTo(20));
        Assert.That(config.CyclomaticComplexityThreshold, Is.EqualTo(4));
        Assert.That(config.MaxRowsPerFile, Is.EqualTo(500));
        Assert.That(config.MaxLinqSteps, Is.EqualTo(2));
        Assert.That(config.MaxIfStatementDots, Is.EqualTo(2));
        Assert.That(config.MaxForEachDots, Is.EqualTo(2));
        Assert.That(config.MaxNestedIfDepth, Is.EqualTo(2));
        Assert.That(config.MaxFilesPerDirectory, Is.EqualTo(6));
        Assert.That(config.SqlKeywordThreshold, Is.EqualTo(3));
    }
}
