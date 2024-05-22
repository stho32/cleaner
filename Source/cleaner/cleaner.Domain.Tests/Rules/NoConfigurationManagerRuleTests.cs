using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules
{
    [TestFixture]
    public class NoConfigurationManagerRuleTests
    {
        private NoConfigurationManagerRule _rule = null!;

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

            Assert.That(messages, Is.Empty);
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

            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo("NoConfigurationManagerRule"));
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

            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo("NoConfigurationManagerRule"));
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

            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(2));
            Assert.That(messages[0]?.RuleId, Is.EqualTo("NoConfigurationManagerRule"));
        }
    }
}
