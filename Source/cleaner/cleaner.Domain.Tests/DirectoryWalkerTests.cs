﻿using cleaner.Domain.Tests.Mocks;

namespace cleaner.Domain.Tests;

using System;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class DirectoryWalkerTests
{
    [Test]
    public void Walk_InvalidPath_ThrowsArgumentException()
    {
        var mockProvider = new MockFileSystemAccessProvider();
        var walker = new DirectoryWalker(_ => { return false; }, mockProvider, "*.cs");

        Assert.Throws<ArgumentException>(() => walker.Walk("invalid_path"));
    }

    [Test]
    public void Walk_ValidPath_CallsCallbackForEachCsFile()
    {
        var mockProvider = new MockFileSystemAccessProvider
        {
            Directories =
            {
                {"root", new List<string> {"root/sub1", "root/sub2"}},
                {"root/sub1", new List<string> {"root/sub1/sub3"}},
                {"root/sub2", new List<string> {"root/sub2/.hidden"}},
                {"root/sub1/sub3", new List<string>()},
                {"root/sub2/.hidden", new List<string>()}
            },
            Files =
            {
                {"root", new List<string> {"root/file1.cs", "root/file2.txt"}},
                {"root/sub1", new List<string> {"root/sub1/file3.cs"}},
                {"root/sub1/sub3", new List<string> {"root/sub1/sub3/file4.cs"}},
                {"root/sub2", new List<string> {"root/sub2/file5.cs"}},
                {"root/sub2/.hidden", new List<string> {"root/sub2/.hidden/file6.cs"}}
            }
        };

        var foundFiles = new List<string>();
        var walker = new DirectoryWalker(filePath =>
        {
            foundFiles.Add(filePath);
            return true;
        }, mockProvider, "*.cs");

        walker.Walk("root");

        Assert.AreEqual(4, foundFiles.Count);
        Assert.Contains("root/file1.cs", foundFiles);
        Assert.Contains("root/sub1/file3.cs", foundFiles);
        Assert.Contains("root/sub1/sub3/file4.cs", foundFiles);
        Assert.Contains("root/sub2/file5.cs", foundFiles);
        Assert.IsFalse(foundFiles.Contains("root/sub2/.hidden/file6.cs"));
    }
}