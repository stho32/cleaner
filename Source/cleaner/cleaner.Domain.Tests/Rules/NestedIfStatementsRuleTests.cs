using cleaner.Domain.Rules;
using cleaner.Domain.Rules.NestedIfStatementsRuleValidation;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules
{
    [TestFixture]
    public class NestedIfStatementsRuleTests
    {
        private NestedIfStatementsRule _rule = null!;

        [SetUp]
        public void Setup()
        {
            _rule = new NestedIfStatementsRule();
        }

        [Test]
        public void NoIfStatements_NoWarning()
        {
            string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                    }
                }
            ";

            var messages = _rule.Validate("TestFile.cs", code);
            Assert.That(messages, Is.Empty);
        }

        [Test]
        public void TwoLevelsNestedIfStatements_NoWarning()
        {
            string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        if (true)
                        {
                            if (false)
                            {
                            }
                        }
                    }
                }
            ";

            var messages = _rule.Validate("TestFile.cs", code);
            Assert.That(messages, Is.Empty);
        }

        [Test]
        public void ThreeLevelsNestedIfStatements_Warning()
        {
            string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        if (true)
                        {
                            if (false)
                            {
                                if (true)
                                {
                                }
                            }
                        }
                    }
                }
            ";

            var messages = _rule.Validate("TestFile.cs", code);
            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
            Assert.That(messages[0]?.RuleName, Is.EqualTo(_rule.Name));
            Assert.That(messages[0]?.ErrorMessage, Does.Contain(
                "Method 'TestMethod' in file 'TestFile.cs' at line 4 has if statements nested more than 2 levels deep."));
        }
    }
}
