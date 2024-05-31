using cleaner.Domain.FileBasedRules.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
    public class SingleDeclarationRuleTests
    {
        private SingleDeclarationRule? _rule;

        [SetUp]
        public void Setup()
        {
            _rule = new SingleDeclarationRule();
        }

        [Test]
        public void SingleDeclarationRule_FileWithOneClass_NoValidationMessages()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = "class Test {}";

            // Act
            var messages = _rule!.Validate(filePath, fileContent);

            // Assert
            Assert.That(messages, Is.Empty);
        }

        [Test]
        public void SingleDeclarationRule_FileWithOneInterface_NoValidationMessages()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = "interface ITest {}";

            // Act
            var messages = _rule!.Validate(filePath, fileContent);

            // Assert
            Assert.That(messages, Is.Empty);
        }

        [Test]
        public void SingleDeclarationRule_FileWithClassAndInterface_ValidationError()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = @"
                class Test {}
                interface ITest {}
            ";

            // Act
            var messages = _rule!.Validate(filePath, fileContent);

            // Assert
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
        }

        [Test]
        public void SingleDeclarationRule_FileWithClassAndEnum_ValidationError()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = @"
                class Test {}
                enum MyEnum { A, B, C }
            ";

            // Act
            var messages = _rule!.Validate(filePath, fileContent);

            // Assert
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
        }

        [Test]
        public void SingleDeclarationRule_FileWithTwoClasses_ValidationError()
        {
            // Arrange
            string filePath = "test.cs";
            string fileContent = @"
                class Test1 {}
                class Test2 {}
            ";

            // Act
            var messages = _rule!.Validate(filePath, fileContent);

            // Assert
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo(_rule.Id));
        }
    }
}
