using cleaner.Domain.FileBasedRules.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
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

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate("TestFile.cs", code, tree, root);
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

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate("TestFile.cs", code, tree, root);
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

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate("TestFile.cs", code, tree, root);
            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
            Assert.That(messages[0]?.RuleName, Is.EqualTo(_rule.Name));
            Assert.That(messages[0]?.ErrorMessage, Does.Contain("SQL detected in non-Repository class 'TestClass'"));
        }
    }
}
