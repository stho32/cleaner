using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

public class ForEachDataSourceRuleTests
{
    [Test]
    public void Validate_NoViolation_DataSourceWithTwoDots()
    {
        string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        var list = new List<string>();

                        foreach (var item in list.Select(x => x.ToUpper()))
                        {
                            // Some code
                        }
                    }
                }
            ";

        var rule = new ForEachDataSourceRule();
        var messages = rule.Validate("TestFile.cs", code);

        Assert.AreEqual(0, messages.Length);
    }

    [Test]
    public void Validate_Violation_DataSourceWithThreeDots()
    {
        string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        var list = new List<string>();

                        foreach (var item in list.Select(x => x.ToUpper()).Where(x => x.Length > 2))
                        {
                            // Some code
                        }
                    }
                }
            ";

        var rule = new ForEachDataSourceRule();
        var messages = rule.Validate("TestFile.cs", code);

        Assert.AreEqual(1, messages.Length);
        Assert.IsTrue(messages[0].ErrorMessage.Contains("TestFile.cs:8"));
    }

    [Test]
    public void Validate_NoViolation_DataSourceWithOneDot()
    {
        string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        var list = new List<string>();

                        foreach (var item in list)
                        {
                            // Some code
                        }
                    }
                }
            ";

        var rule = new ForEachDataSourceRule();
        var messages = rule.Validate("TestFile.cs", code);

        Assert.AreEqual(0, messages.Length);
    }
}