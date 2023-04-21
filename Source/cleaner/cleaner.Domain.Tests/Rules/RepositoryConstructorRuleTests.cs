using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class RepositoryConstructorRuleTests
{
    private RepositoryConstructorRule _rule = null!;

    [SetUp]
    public void Setup()
    {
        _rule = new RepositoryConstructorRule();
    }

    [Test]
    public void NonRepositoryClass_NoWarning()
    {
        string code = @"
                public class TestClass
                {
                    public TestClass(IDatabaseAccessor db)
                    {
                    }
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void RepositoryClassWithRequiredConstructor_NoWarning()
    {
        string code = @"
                public class TestRepository
                {
                    public TestRepository(IDatabaseAccessor db)
                    {
                    }
                }
            ";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void RepositoryClassWithoutRequiredConstructor_Warning()
    {
        string code = @"
                public class TestRepository
                {
                    public TestRepository()
                    {
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
            "Class 'TestRepository' in file 'TestFile.cs' at line 2 should have a constructor with at least one parameter of type 'IDatabaseAccessor'.",
            messages[0]?.ErrorMessage);
    }
}