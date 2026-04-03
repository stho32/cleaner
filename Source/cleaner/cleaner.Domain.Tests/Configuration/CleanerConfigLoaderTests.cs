using cleaner.Domain.Configuration;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Configuration;

public class CleanerConfigLoaderTests
{
    private string _tempDir = null!;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void Teardown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Test]
    public void Load_NoConfigFile_ReturnsDefaults()
    {
        var config = CleanerConfigLoader.Load(_tempDir);

        Assert.That(config.MethodLengthMaxSemicolons, Is.EqualTo(20));
        Assert.That(config.MaxRowsPerFile, Is.EqualTo(500));
    }

    [Test]
    public void Load_ValidConfigFile_ParsesValues()
    {
        File.WriteAllText(Path.Combine(_tempDir, ".cleaner.json"),
            """{"MethodLengthMaxSemicolons": 42, "MaxRowsPerFile": 100}""");

        var config = CleanerConfigLoader.Load(_tempDir);

        Assert.That(config.MethodLengthMaxSemicolons, Is.EqualTo(42));
        Assert.That(config.MaxRowsPerFile, Is.EqualTo(100));
        Assert.That(config.CyclomaticComplexityThreshold, Is.EqualTo(4));
    }

    [Test]
    public void Load_InvalidJson_ReturnsDefaults()
    {
        File.WriteAllText(Path.Combine(_tempDir, ".cleaner.json"), "not valid json");

        var config = CleanerConfigLoader.Load(_tempDir);

        Assert.That(config.MethodLengthMaxSemicolons, Is.EqualTo(20));
    }

    [Test]
    public void Load_CaseInsensitive_ParsesValues()
    {
        File.WriteAllText(Path.Combine(_tempDir, ".cleaner.json"),
            """{"methodlengthmaxsemicolons": 15}""");

        var config = CleanerConfigLoader.Load(_tempDir);

        Assert.That(config.MethodLengthMaxSemicolons, Is.EqualTo(15));
    }
}
