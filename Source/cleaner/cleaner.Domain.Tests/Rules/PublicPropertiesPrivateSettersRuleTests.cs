using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class PublicPropertiesPrivateSettersRuleTests
{
    private PublicPropertiesPrivateSettersRule _rule;

    [SetUp]
    public void Setup()
    {
        _rule = new PublicPropertiesPrivateSettersRule();
    }

    [Test]
    public void Validate_PublicPropertyWithPrivateSetter_ShouldNotReturnWarning()
    {
        string code = @"
            using System;

            class TestClass
            {
                public int TestProperty { get; private set; }
            }";

        var messages = _rule.Validate("TestFile.cs", code);

        Assert.IsEmpty(messages);
    }

    [Test]
    public void Validate_PublicPropertyWithPublicSetter_ShouldReturnWarning()
    {
        string code = @"
            using System;

            class TestClass
            {
                public int TestProperty { get; set; }
            }";

        var messages = _rule.Validate("TestFile.cs", code);

        Assert.IsNotEmpty(messages);
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual("PublicPropertiesPrivateSettersRule", messages[0].RuleId);
    }
}