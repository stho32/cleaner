using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.Tests.Mocks;
using NUnit.Framework;

namespace cleaner.Domain.Tests.DirectoryTraversal;

public class DirectoryFilterTests
{
    private MockFileSystemAccessProvider _fs = null!;

    [SetUp]
    public void Setup()
    {
        _fs = new MockFileSystemAccessProvider();
    }

    [Test]
    public void ShouldIgnore_BinFolderLinux_True()
    {
        Assert.That(DirectoryFilter.ShouldIgnore("/src/bin/Debug", _fs), Is.True);
    }

    [Test]
    public void ShouldIgnore_ObjFolderLinux_True()
    {
        Assert.That(DirectoryFilter.ShouldIgnore("/src/obj/Release", _fs), Is.True);
    }

    [Test]
    public void ShouldIgnore_BinFolderWindows_True()
    {
        Assert.That(DirectoryFilter.ShouldIgnore(@"C:\src\bin\Debug", _fs), Is.True);
    }

    [Test]
    public void ShouldIgnore_ObjFolderWindows_True()
    {
        Assert.That(DirectoryFilter.ShouldIgnore(@"C:\src\obj\Release", _fs), Is.True);
    }

    [Test]
    public void ShouldIgnore_DotFolder_True()
    {
        Assert.That(DirectoryFilter.ShouldIgnore("/src/.git", _fs), Is.True);
    }

    [Test]
    public void ShouldIgnore_NormalFolder_False()
    {
        Assert.That(DirectoryFilter.ShouldIgnore("/src/Services", _fs), Is.False);
    }

    [Test]
    public void ShouldIgnore_VsFolder_True()
    {
        Assert.That(DirectoryFilter.ShouldIgnore(@"C:\src\.vs", _fs), Is.True);
    }
}
