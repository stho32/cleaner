using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class NestedIfStatementsRuleTests
{
    private NestedIfStatementsRule _rule = null!;

    [SetUp]
    public void Setup()
    {
        _rule = new NestedIfStatementsRule();
    }

    [Test]
    public void NoIfStatements_NoWarning()
    {
        string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                    }
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void TwoLevelsNestedIfStatements_NoWarning()
    {
        string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        if (true)
                        {
                            if (false)
                            {
                            }
                        }
                    }
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void ThreeLevelsNestedIfStatements_Warning()
    {
        string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        if (true)
                        {
                            if (false)
                            {
                                if (true)
                                {
                                }
                            }
                        }
                    }
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsNotEmpty(messages);
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(Severity.Warning, messages[0]?.Severity);
        Assert.AreEqual(_rule.Id, messages[0]?.RuleId);
        Assert.AreEqual(_rule.Name, messages[0]?.RuleName);
        StringAssert.Contains(
            "Method 'TestMethod' in file 'TestFile.cs' at line 4 has if statements nested more than 2 levels deep.",
            messages[0]?.ErrorMessage);
    }
}