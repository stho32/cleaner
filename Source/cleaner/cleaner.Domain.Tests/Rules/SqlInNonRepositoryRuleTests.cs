using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class SqlInNonRepositoryRuleTests
{
    private SqlInNonRepositoryRule _rule = null!;

    [SetUp]
    public void Setup()
    {
        _rule = new SqlInNonRepositoryRule();
    }

    [Test]
    public void NoSql_NoWarning()
    {
        string code = @"
                public class TestClass
                {
                    public void DoSomething()
                    {
                        string message = ""Hello, World!"";
                    }
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void SqlInRepositoryClass_NoWarning()
    {
        string code = @"
                public class TestRepository
                {
                    public void GetData()
                    {
                        string sql = ""SELECT * FROM Users"";
                    }
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void SqlInNonRepositoryClass_Warning()
    {
        string code = @"
                public class TestClass
                {
                    public void GetData()
                    {
                        string sql = ""SELECT TOP 10 * FROM Users"";
                    }
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsNotEmpty(messages);
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(_rule.Id, messages[0]?.RuleId);
        Assert.AreEqual(_rule.Name, messages[0]?.RuleName);
        StringAssert.Contains("SQL detected in non-Repository class 'TestClass'", messages[0]?.ErrorMessage);
    }
}