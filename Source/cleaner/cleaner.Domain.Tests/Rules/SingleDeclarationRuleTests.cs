using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

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
        Assert.IsEmpty(messages);
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
        Assert.IsEmpty(messages);
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
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(_rule.Id, messages[0]?.RuleId);
        Assert.AreEqual(Severity.Error, messages[0]?.Severity);
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
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(_rule.Id, messages[0]?.RuleId);
        Assert.AreEqual(Severity.Error, messages[0]?.Severity);
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
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(_rule.Id, messages[0]?.RuleId);
        Assert.AreEqual(Severity.Error, messages[0]?.Severity);
    }
}