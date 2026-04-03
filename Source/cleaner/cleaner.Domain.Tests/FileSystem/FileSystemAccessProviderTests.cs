using cleaner.Domain.FileSystem;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileSystem;

public class FileSystemAccessProviderTests
{
    private string _tempDir = null!;
    private FileSystemAccessProvider _provider = null!;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _provider = new FileSystemAccessProvider();
    }

    [TearDown]
    public void Teardown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Test]
    public void DirectoryExists_ExistingDir_True()
    {
        Assert.That(_provider.DirectoryExists(_tempDir), Is.True);
    }

    [Test]
    public void DirectoryExists_NonExistingDir_False()
    {
        Assert.That(_provider.DirectoryExists(Path.Combine(_tempDir, "nonexistent")), Is.False);
    }

    [Test]
    public void GetDirectories_ReturnsSubdirectories()
    {
        Directory.CreateDirectory(Path.Combine(_tempDir, "sub1"));
        Directory.CreateDirectory(Path.Combine(_tempDir, "sub2"));

        var dirs = _provider.GetDirectories(_tempDir).ToArray();

        Assert.That(dirs, Has.Length.EqualTo(2));
    }

    [Test]
    public void GetFiles_ReturnsMatchingFiles()
    {
        File.WriteAllText(Path.Combine(_tempDir, "test.cs"), "class Foo {}");
        File.WriteAllText(Path.Combine(_tempDir, "readme.md"), "# Test");

        var csFiles = _provider.GetFiles(_tempDir, "*.cs").ToArray();

        Assert.That(csFiles, Has.Length.EqualTo(1));
        Assert.That(csFiles[0], Does.EndWith("test.cs"));
    }

    [Test]
    public void GetFileContent_ReturnsContent()
    {
        var file = Path.Combine(_tempDir, "test.cs");
        File.WriteAllText(file, "public class Foo {}");

        var content = _provider.GetFileContent(file);

        Assert.That(content, Is.EqualTo("public class Foo {}"));
    }

    [Test]
    public void GetFileName_ReturnsLastSegment()
    {
        var result = _provider.GetFileName(Path.Combine(_tempDir, "subfolder"));

        Assert.That(result, Is.EqualTo("subfolder"));
    }

    [Test]
    public void GetLastWriteTimeUtc_ReturnsValidTime()
    {
        var file = Path.Combine(_tempDir, "test.cs");
        File.WriteAllText(file, "test");

        var time = _provider.GetLastWriteTimeUtc(file);

        Assert.That(time, Is.GreaterThan(DateTime.MinValue));
        Assert.That(time.Kind, Is.EqualTo(DateTimeKind.Utc));
    }
}
