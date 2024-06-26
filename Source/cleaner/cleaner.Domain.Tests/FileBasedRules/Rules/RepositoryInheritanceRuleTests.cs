using cleaner.Domain.FileBasedRules.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
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
            Assert.That(messages, Is.Empty);
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
            Assert.That(messages, Is.Empty);
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
            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
            Assert.That(messages[0]?.RuleName, Is.EqualTo(_rule.Name));
            Assert.That(messages[0]?.ErrorMessage, Does.Contain("Class 'TestRepository' in file 'TestFile.cs' at line 6 should not inherit from another class."));
        }
    }
}