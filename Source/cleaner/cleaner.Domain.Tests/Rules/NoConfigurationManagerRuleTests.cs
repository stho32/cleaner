using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class NoConfigurationManagerRuleTests
{
    private NoConfigurationManagerRule _rule;

    [SetUp]
    public void Setup()
    {
        _rule = new NoConfigurationManagerRule();
    }

    [Test]
    public void Validate_NoConfigurationManager_ShouldNotReturnWarning()
    {
        string code = @"
            using System;

            namespace TestNamespace
            {
                class TestClass
                {
                    public void SomeMethod()
                    {
                        Console.WriteLine(""Hello World!"");
                    }
                }
            }";

        var messages = _rule.Validate("TestClass.cs", code);

        Assert.IsEmpty(messages);
    }

    [Test]
    public void Validate_UsesConfigurationManager_ShouldReturnError()
    {
        string code = @"
            using System.Configuration;

            namespace TestNamespace
            {
                class TestClass
                {
                    public void SomeMethod()
                    {
                        var connectionString = ConfigurationManager.ConnectionStrings[""MyConnectionString""].ConnectionString;
                    }
                }
            }";

        var messages = _rule.Validate("TestClass.cs", code);

        Assert.IsNotEmpty(messages);
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(Severity.Warning, messages[0].Severity);
        Assert.AreEqual("NoConfigurationManagerRule", messages[0].RuleId);
    }

    [Test]
    public void Validate_UsesWebConfigurationManager_ShouldReturnError()
    {
        string code = @"
            using System.Web.Configuration;

            namespace TestNamespace
            {
                class TestClass
                {
                    public void SomeMethod()
                    {
                        var connectionString = WebConfigurationManager.ConnectionStrings[""MyConnectionString""].ConnectionString;
                    }
                }
            }";

        var messages = _rule.Validate("TestClass.cs", code);

        Assert.IsNotEmpty(messages);
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(Severity.Warning, messages[0].Severity);
        Assert.AreEqual("NoConfigurationManagerRule", messages[0].RuleId);
    }

    [Test]
    public void Validate_UsesBothConfigurationManagers_ShouldReturnError()
    {
        string code = @"
            using System.Configuration;
            using System.Web.Configuration;

            namespace TestNamespace
            {
                class TestClass
                {
                    public void SomeMethod()
                    {
                        var connectionString1 = ConfigurationManager.ConnectionStrings[""MyConnectionString""].ConnectionString;
                        var connectionString2 = WebConfigurationManager.ConnectionStrings[""MyConnectionString""].ConnectionString;
                    }
                }
            }";

        var messages = _rule.Validate("TestClass.cs", code);

        Assert.IsNotEmpty(messages);
        Assert.AreEqual(2, messages.Length);
        Assert.AreEqual(Severity.Warning, messages[0].Severity);
        Assert.AreEqual("NoConfigurationManagerRule", messages[0].RuleId);
    }
}