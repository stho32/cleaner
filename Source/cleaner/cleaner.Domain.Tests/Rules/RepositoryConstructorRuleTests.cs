using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules
{
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
            Assert.That(messages, Is.Empty);
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
            Assert.That(messages, Is.Empty);
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
            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
            Assert.That(messages[0]?.RuleName, Is.EqualTo(_rule.Name));
            Assert.That(messages[0]?.ErrorMessage, Does.Contain("Class 'TestRepository' in file 'TestFile.cs' at line 2 should have a constructor with at least one parameter of type 'IDatabaseAccessor'."));
        }
    }
}
