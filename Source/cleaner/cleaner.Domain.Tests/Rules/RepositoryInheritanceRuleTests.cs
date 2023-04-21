using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class RepositoryInheritanceRuleTests
{
    private RepositoryInheritanceRule _rule = null!;

    [SetUp]
    public void Setup()
    {
        _rule = new RepositoryInheritanceRule();
    }

    [Test]
    public void NonRepositoryClassWithInheritance_NoWarning()
    {
        string code = @"
                public class MyBaseClass
                {
                }

                public class TestClass : MyBaseClass
                {
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void RepositoryClassWithoutInheritance_NoWarning()
    {
        string code = @"
                public class TestRepository
                {
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void RepositoryClassWithInheritance_Warning()
    {
        string code = @"
                public class MyBaseClass
                {
                }

                public class TestRepository : MyBaseClass
                {
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsNotEmpty(messages);
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(Severity.Warning, messages[0]?.Severity);
        Assert.AreEqual(_rule.Id, messages[0]?.RuleId);
        Assert.AreEqual(_rule.Name, messages[0]?.RuleName);
        StringAssert.Contains("Class 'TestRepository' in file 'TestFile.cs' at line 6 should not inherit from another class.",
            messages[0]?.ErrorMessage);
    }
}