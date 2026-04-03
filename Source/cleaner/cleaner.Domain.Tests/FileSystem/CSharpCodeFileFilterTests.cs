using cleaner.Domain.FileSystem;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileSystem;

public class CSharpCodeFileFilterTests
{
    private CSharpCodeFileFilter _filter = null!;

    [SetUp]
    public void Setup()
    {
        _filter = new CSharpCodeFileFilter();
    }

    [Test]
    public void IsValidFilename_CsFile_True()
    {
        Assert.That(_filter.IsValidFilename("MyClass.cs"), Is.True);
    }

    [Test]
    public void IsValidFilename_CsFileWithPath_True()
    {
        Assert.That(_filter.IsValidFilename("/src/MyClass.cs"), Is.True);
    }

    [Test]
    public void IsValidFilename_DesignerFile_False()
    {
        Assert.That(_filter.IsValidFilename("Form1.Designer.cs"), Is.False);
    }

    [Test]
    public void IsValidFilename_NonCsFile_False()
    {
        Assert.That(_filter.IsValidFilename("readme.md"), Is.False);
    }

    [Test]
    public void IsValidFilename_TxtFile_False()
    {
        Assert.That(_filter.IsValidFilename("data.txt"), Is.False);
    }

    [Test]
    public void IsValidContent_WithContent_True()
    {
        Assert.That(_filter.IsValidContent("public class Foo {}"), Is.True);
    }

    [Test]
    public void IsValidContent_Null_False()
    {
        Assert.That(_filter.IsValidContent(null), Is.False);
    }

    [Test]
    public void IsValidContent_Empty_False()
    {
        Assert.That(_filter.IsValidContent(""), Is.False);
    }
}
