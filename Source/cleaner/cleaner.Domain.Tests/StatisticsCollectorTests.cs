using NUnit.Framework;

namespace cleaner.Domain.Tests;

public class StatisticsCollectorTests
{
    [Test]
    public void InitialState_AllZeros()
    {
        var collector = new StatisticsCollector();
        var stats = collector.GetStatistics();

        Assert.That(stats.filesScanned, Is.EqualTo(0));
        Assert.That(stats.foldersScanned, Is.EqualTo(0));
        Assert.That(stats.filesWithProblems, Is.EqualTo(0));
        Assert.That(stats.foldersWithProblems, Is.EqualTo(0));
    }

    [Test]
    public void ScanningFile_IncrementsCounter()
    {
        var collector = new StatisticsCollector();
        collector.ScanningFile();
        collector.ScanningFile();
        collector.ScanningFile();

        var stats = collector.GetStatistics();
        Assert.That(stats.filesScanned, Is.EqualTo(3));
    }

    [Test]
    public void FoundFileWithProblems_IncrementsCounter()
    {
        var collector = new StatisticsCollector();
        collector.FoundFileWithProblems();
        collector.FoundFileWithProblems();

        var stats = collector.GetStatistics();
        Assert.That(stats.filesWithProblems, Is.EqualTo(2));
    }

    [Test]
    public void ScanningFolder_IncrementsCounter()
    {
        var collector = new StatisticsCollector();
        collector.ScanningFolder();

        var stats = collector.GetStatistics();
        Assert.That(stats.foldersScanned, Is.EqualTo(1));
    }

    [Test]
    public void FoundFolderWithProblems_IncrementsCounter()
    {
        var collector = new StatisticsCollector();
        collector.FoundFolderWithProblems();

        var stats = collector.GetStatistics();
        Assert.That(stats.foldersWithProblems, Is.EqualTo(1));
    }

    [Test]
    public void MixedOperations_AllCountersCorrect()
    {
        var collector = new StatisticsCollector();
        collector.ScanningFile();
        collector.ScanningFile();
        collector.FoundFileWithProblems();
        collector.ScanningFolder();
        collector.ScanningFolder();
        collector.ScanningFolder();
        collector.FoundFolderWithProblems();
        collector.FoundFolderWithProblems();

        var stats = collector.GetStatistics();
        Assert.That(stats.filesScanned, Is.EqualTo(2));
        Assert.That(stats.filesWithProblems, Is.EqualTo(1));
        Assert.That(stats.foldersScanned, Is.EqualTo(3));
        Assert.That(stats.foldersWithProblems, Is.EqualTo(2));
    }
}
