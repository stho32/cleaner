using cleaner.Domain.FileBasedRules.Rules.NestedIfStatementsRuleValidation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
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

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate("TestFile.cs", code, tree, root);
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

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate("TestFile.cs", code, tree, root);
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

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate("TestFile.cs", code, tree, root);
            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
            Assert.That(messages[0]?.RuleName, Is.EqualTo(_rule.Name));
            Assert.That(messages[0]?.ErrorMessage, Does.Contain(
                "Method 'TestMethod' in file 'TestFile.cs' at line 4 has if statements nested more than 2 levels deep."));
        }
    }
}
