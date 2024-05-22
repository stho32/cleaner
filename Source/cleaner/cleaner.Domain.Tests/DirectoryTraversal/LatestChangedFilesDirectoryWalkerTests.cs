using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.Tests.Mocks;
using NUnit.Framework;

namespace cleaner.Domain.Tests.DirectoryTraversal
{
    [TestFixture]
    public class LatestChangedFilesDirectoryWalkerTests
    {
        private MockFileSystemAccessProvider SetUpMockFileSystemAccessProvider()
        {
            return new MockFileSystemAccessProvider
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
                },
                FileLastWriteTimes =
                {
                    {"root/file1.cs", DateTime.UtcNow.AddHours(-1)},
                    {"root/file2.txt", DateTime.UtcNow.AddHours(-2)},
                    {"root/sub1/file3.cs", DateTime.UtcNow.AddHours(-3)},
                    {"root/sub1/sub3/file4.cs", DateTime.UtcNow.AddHours(-4)},
                    {"root/sub2/file5.cs", DateTime.UtcNow.AddHours(-5)},
                    {"root/sub2/.hidden/file6.cs", DateTime.UtcNow.AddHours(-6)}
                }
            };
        }

        [Test]
        public void Walk_InvalidPath_ThrowsArgumentException()
        {
            var mockProvider = new MockFileSystemAccessProvider();
            var walker = new LatestChangedFilesDirectoryWalker(3);

            Assert.That(() => walker.Walk(_ => false, mockProvider, "*.cs", "invalid_path"), Throws.ArgumentException);
        }

        [Test]
        public void Walk_ValidPath_CallsCallbackForLatestNChangedCsFiles()
        {
            var mockProvider = SetUpMockFileSystemAccessProvider();
            var foundFiles = new List<string>();
            var walker = new LatestChangedFilesDirectoryWalker(3);

            walker.Walk(filePath =>
            {
                foundFiles.Add(filePath);
                return true;
            }, mockProvider, "*.cs", "root");

            AssertFoundFiles(foundFiles);
        }

        private void AssertFoundFiles(List<string> foundFiles)
        {
            Assert.That(foundFiles.Count, Is.EqualTo(3));
            Assert.That(foundFiles, Does.Contain("root/file1.cs"));
            Assert.That(foundFiles, Does.Contain("root/sub1/file3.cs"));
            Assert.That(foundFiles, Does.Contain("root/sub1/sub3/file4.cs"));
            Assert.That(foundFiles, Does.Not.Contain("root/sub2/file5.cs"));
            Assert.That(foundFiles, Does.Not.Contain("root/sub2/.hidden/file6.cs"));
        }
    }
}
