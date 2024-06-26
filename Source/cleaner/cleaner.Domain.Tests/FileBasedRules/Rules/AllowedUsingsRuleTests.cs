using cleaner.Domain.FileBasedRules.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules;

[TestFixture]
public class AllowedUsingsRuleTests
{
    private AllowedUsingsRule _rule = null!;

    [SetUp]
    public void Setup()
    {
        var allowedUsings = new HashSet<string>
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            // Add more allowed usings here
        };

        _rule = new AllowedUsingsRule(allowedUsings);
    }

    [Test]
    public void Validate_AllowedUsings_ShouldNotReturnWarning()
    {
        string code = @"
                using System;
                using System.Collections.Generic;
                using System.Linq;

                namespace TestNamespace
                {
                    class TestClass
                    {
                    }
                }";

        var messages = _rule.Validate("TestFile.cs", code);

        Assert.That(messages, Is.Empty);
    }

    [Test]
    public void Validate_DisallowedUsings_ShouldReturnWarning()
    {
        string code = @"
                using Disallowed.Text;

                namespace TestNamespace
                {
                    class TestClass
                    {
                    }
                }";

        var messages = _rule.Validate("TestFile.cs", code);

        Assert.That(messages, Is.Not.Empty);
        Assert.That(messages.Length, Is.EqualTo(1));
        Assert.That(messages[0]?.RuleId, Is.EqualTo("AllowedUsingsRule"));
    }

    [Test]
    public void Validate_SubNamespaceOfRootNamespace_ShouldNotReturnWarning()
    {
        string code = @"
                using System;
                using System.Collections.Generic;
                using TestNamespace.SubNamespace;

                namespace TestNamespace
                {
                    class TestClass
                    {
                    }
                }";

        var messages = _rule.Validate("TestFile.cs", code);

        Assert.That(messages, Is.Empty);
    }
}