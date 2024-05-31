using cleaner.Domain.FileBasedRules.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
    public class ForEachDataSourceRuleTests
    {
        [Test]
        public void Validate_NoViolation_DataSourceWithTwoDots()
        {
            string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        var list = new List<string>();

                        foreach (var item in list.Select(x => x.ToUpper()))
                        {
                            // Some code
                        }
                    }
                }
            ";

            var rule = new ForEachDataSourceRule();
            var messages = rule.Validate("TestFile.cs", code);

            Assert.That(messages.Length, Is.EqualTo(0));
        }

        [Test]
        public void Validate_Violation_DataSourceWithThreeDots()
        {
            string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        var list = new List<string>();

                        foreach (var item in list.Select(x => x.ToUpper()).Where(x => x.Length > 2))
                        {
                            // Some code
                        }
                    }
                }
            ";

            var rule = new ForEachDataSourceRule();
            var messages = rule.Validate("TestFile.cs", code);

            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0].ErrorMessage, Contains.Substring("'TestFile.cs':8"));
        }

        [Test]
        public void Validate_NoViolation_DataSourceWithOneDot()
        {
            string code = @"
                public class TestClass
                {
                    public void TestMethod()
                    {
                        var list = new List<string>();

                        foreach (var item in list)
                        {
                            // Some code
                        }
                    }
                }
            ";

            var rule = new ForEachDataSourceRule();
            var messages = rule.Validate("TestFile.cs", code);

            Assert.That(messages.Length, Is.EqualTo(0));
        }
    }
}
