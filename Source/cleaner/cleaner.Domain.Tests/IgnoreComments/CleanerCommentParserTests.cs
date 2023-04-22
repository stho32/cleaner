using cleaner.Domain.IgnoreComments;
using NUnit.Framework;

namespace cleaner.Domain.Tests.IgnoreComments;

[TestFixture]
public class CleanerCommentParserTests
{
    [Test]
    public void GetIgnoredRuleIds_NoComments_ReturnsEmptyHashSet()
    {
        string code = "public class TestClass { }";

        HashSet<string> ignoredRuleIds = CleanerCommentParser.GetIgnoredRuleIds(code);

        Assert.IsEmpty(ignoredRuleIds);
    }

    [Test]
    public void GetIgnoredRuleIds_OneIgnoreComment_ReturnsHashSetWithOneElement()
    {
        string code = "// cleaner: ignore SqlInNonRepositoryRule\npublic class TestClass { }";

        HashSet<string> ignoredRuleIds = CleanerCommentParser.GetIgnoredRuleIds(code);

        Assert.AreEqual(1, ignoredRuleIds.Count);
        Assert.IsTrue(ignoredRuleIds.Contains("SqlInNonRepositoryRule"));
    }

    [Test]
    public void GetIgnoredRuleIds_MultipleIgnoreComments_ReturnsHashSetWithMultipleElements()
    {
        string code = "// cleaner: ignore SqlInNonRepositoryRule\n// cleaner: ignore MethodLengthRule\npublic class TestClass { }";

        HashSet<string> ignoredRuleIds = CleanerCommentParser.GetIgnoredRuleIds(code);

        Assert.AreEqual(2, ignoredRuleIds.Count);
        Assert.IsTrue(ignoredRuleIds.Contains("SqlInNonRepositoryRule"));
        Assert.IsTrue(ignoredRuleIds.Contains("MethodLengthRule"));
    }

    [Test]
    public void GetIgnoredRuleIds_InvalidComment_IgnoresInvalidComment()
    {
        string code = "// cleaner: ignore SqlInNonRepositoryRule\n// cleaner: invalid comment\npublic class TestClass { }";

        HashSet<string> ignoredRuleIds = CleanerCommentParser.GetIgnoredRuleIds(code);

        Assert.AreEqual(1, ignoredRuleIds.Count);
        Assert.IsTrue(ignoredRuleIds.Contains("SqlInNonRepositoryRule"));
    }
}
