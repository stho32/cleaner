using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules
{
    // cleaner: ignore SqlInNonRepositoryRule
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
            Assert.That(messages, Is.Empty);
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
            Assert.That(messages, Is.Empty);
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
            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
            Assert.That(messages[0]?.RuleName, Is.EqualTo(_rule.Name));
            Assert.That(messages[0]?.ErrorMessage, Does.Contain("SQL detected in non-Repository class 'TestClass'"));
        }
    }
}
