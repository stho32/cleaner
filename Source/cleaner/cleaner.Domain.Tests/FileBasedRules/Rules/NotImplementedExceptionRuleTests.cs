using cleaner.Domain.FileBasedRules.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
    public class NotImplementedExceptionRuleTests
    {
        private NotImplementedExceptionRule _rule = null!;

        [SetUp]
        public void Setup()
        {
            _rule = new NotImplementedExceptionRule();
        }

        [Test]
        public void NotImplementedExceptionRule_NoNotImplementedExceptions_NoValidationMessages()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = "class Test { void DoSomething() { } }";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(messages, Is.Empty);
        }

        [Test]
        public void NotImplementedExceptionRule_OneNotImplementedException_ValidationWarning()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = "class Test { void DoSomething() { throw new NotImplementedException(); } }";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
        }

        [Test]
        public void NotImplementedExceptionRule_TwoNotImplementedExceptions_ValidationWarning()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = @"
                class Test {
                    void DoSomething() { throw new NotImplementedException(); }
                    void DoAnotherThing() { throw new NotImplementedException(); }
                }";

            // Act
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate(filePath, fileContent, tree, root);

            // Assert
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
        }
    }
}