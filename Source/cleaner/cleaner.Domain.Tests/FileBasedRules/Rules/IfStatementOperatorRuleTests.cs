using cleaner.Domain.FileBasedRules.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
    public class IfStatementOperatorRuleTests
    {
        private IRule _ifStatementOperatorRule = null!;

        [SetUp]
        public void Setup()
        {
            _ifStatementOperatorRule = new IfStatementOperatorRule();
        }

        [Test]
        public void Validate_NoIfStatements_ShouldReturnNoMessages()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = "public class TestClass { }";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var result = _ifStatementOperatorRule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_IfStatementWithFunctionCall_ShouldReturnNoMessages()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = @"
public class TestClass
{
    public void TestMethod(bool value)
    {
        if (IsTrue(value))
        {
            // Do something
        }
    }

    public bool IsTrue(bool input)
    {
        return input;
    }
}";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var result = _ifStatementOperatorRule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_IfStatementWithOperator_ShouldReturnWarning()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = @"
public class TestClass
{
    public void TestMethod(int a, int b)
    {
        if (a > b)
        {
            // Do something
        }
    }
}";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var result = _ifStatementOperatorRule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(result?.Length, Is.EqualTo(1));
            Assert.That(result?[0].RuleId, Is.EqualTo("IfStatementOperatorRule"));
            Assert.That(result?[0].RuleName, Is.EqualTo("If Statement Operator Rule"));
            Assert.That(result?[0].ErrorMessage, Does.Contain("contains operators, which is not allowed"));
        }

        [Test]
        public void IfStatementOperatorRule_AllowsSimpleNullComparison()
        {
            // Arrange
            var ifStatementOperatorRule = new IfStatementOperatorRule();
            string testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            object obj = null;
            if (obj == null)
            {
                // do something
            }
        }
    }
}";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(testCode);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = ifStatementOperatorRule.Validate("TestClass.cs", testCode, tree, root);

            // Assert
            Assert.That(messages, Is.Empty, "The IfStatementOperatorRule should allow simple null comparisons.");
        }
    }
}
