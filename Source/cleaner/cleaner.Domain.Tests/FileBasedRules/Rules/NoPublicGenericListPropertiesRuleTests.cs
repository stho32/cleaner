using cleaner.Domain.FileBasedRules.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
    [TestFixture]
    public class NoPublicGenericListPropertiesRuleTests
    {
        private NoPublicGenericListPropertiesRule _rule = null!;

        [SetUp]
        public void Setup()
        {
            _rule = new NoPublicGenericListPropertiesRule();
        }

        [Test]
        public void Validate_NoPublicListProperties_ShouldNotReturnWarning()
        {
            string code = @"
            namespace TestNamespace
            {
                class TestClass
                {
                    private List<int> privateList = new List<int>();
                    internal List<int> internalList = new List<int>();
                    protected List<int> protectedList = new List<int>();
                    public int publicInt = 42;
                    public string publicString { get; }
                    public TestClass()
                    {
                        publicString = ""Hello world!"";
                    }
                }
            }";

            var messages = _rule.Validate("TestClass.cs", code);

            Assert.That(messages, Is.Empty);
        }

        [Test]
        public void Validate_PublicListProperty_ShouldReturnWarning()
        {
            string code = @"
            namespace TestNamespace
            {
                class TestClass
                {
                    public List<int> PublicList { get; }
                }
            }";

            var messages = _rule.Validate("TestClass.cs", code);

            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo("NoPublicGenericListPropertiesRule"));
        }

        [Test]
        public void Validate_GenericListProperty_ShouldReturnWarning()
        {
            string code = @"
            using System.Collections.Generic;
            namespace TestNamespace
            {
                class TestClass
                {
                    public List<string> PublicList { get; set; }
                }
            }";

            var messages = _rule.Validate("TestClass.cs", code);

            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo("NoPublicGenericListPropertiesRule"));
        }
    }
}
