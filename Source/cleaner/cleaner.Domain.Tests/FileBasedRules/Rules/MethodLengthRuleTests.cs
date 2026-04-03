using cleaner.Domain.FileBasedRules.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
    public class MethodLengthRuleTests
    {
        private IRule _methodLengthRule = null!;

        [SetUp]
        public void Setup()
        {
            _methodLengthRule = new MethodLengthRule(10);
        }

        [Test]
        public void Validate_NoMethods_ShouldReturnNoMessages()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = "public class TestClass { }";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var result = _methodLengthRule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_MethodWithFewerSemicolons_ShouldReturnNoMessages()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = @"
public class TestClass
{
    public void TestMethod()
    {
        int a = 1;
        int b = 2;
        int c = 3;
    }
}";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var result = _methodLengthRule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_MethodWithMoreSemicolons_ShouldReturnWarning()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = @"
public class TestClass
{
    public void TestMethod()
    {
        int a = 1; int b = 2; int c = 3; int d = 4; int e = 5;
        int f = 6; int g = 7; int h = 8; int i = 9; int j = 10; int k = 11;
    }
}";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var result = _methodLengthRule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(result?.Length, Is.EqualTo(1));
            Assert.That(result![0]?.RuleId, Is.EqualTo("MethodLengthRule"));
            Assert.That(result[0]?.RuleName, Is.EqualTo("Method Length Rule"));
            Assert.That(result[0]?.ErrorMessage, Does.Contain("more than the allowed limit of 10"));
        }
    }
}
