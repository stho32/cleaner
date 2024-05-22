using cleaner.Domain.DirectoryTraversal;
using cleaner.Domain.Tests.Mocks;
using NUnit.Framework;

namespace cleaner.Domain.Tests.DirectoryTraversal
{
    [TestFixture]
    public class RecursiveDirectoryWalkerTests
    {
        [Test]
        public void Walk_InvalidPath_ThrowsArgumentException()
        {
            var mockProvider = new MockFileSystemAccessProvider();
            var walker = new RecursiveDirectoryWalker();

            Assert.That(() => walker.Walk(_ => false, mockProvider, "*.cs", "invalid_path"), Throws.ArgumentException);
        }

        [Test]
        public void Walk_ValidPath_CallsCallbackForEachCsFile()
        {
            var mockProvider = SetUpMockFileSystemAccessProvider();
            var foundFiles = new List<string>();
            var walker = new RecursiveDirectoryWalker();

            walker.Walk(filePath =>
            {
                foundFiles.Add(filePath);
                return true;
            }, mockProvider, "*.cs", "root");

            AssertFoundFiles(foundFiles);
        }

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
                }
            };
        }

        private void AssertFoundFiles(List<string> foundFiles)
        {
            Assert.That(foundFiles.Count, Is.EqualTo(4));
            Assert.That(foundFiles, Does.Contain("root/file1.cs"));
            Assert.That(foundFiles, Does.Contain("root/sub1/file3.cs"));
            Assert.That(foundFiles, Does.Contain("root/sub1/sub3/file4.cs"));
            Assert.That(foundFiles, Does.Contain("root/sub2/file5.cs"));
            Assert.That(foundFiles, Does.Not.Contain("root/sub2/.hidden/file6.cs"));
        }
    }
}
