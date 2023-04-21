using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

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
        var messages = _rule.Validate(filePath, fileContent);

        // Assert
        Assert.IsEmpty(messages);
    }

    [Test]
    public void NotImplementedExceptionRule_OneNotImplementedException_ValidationWarning()
    {
        // Arrange
        string filePath = "test.cs";
        string fileContent = "class Test { void DoSomething() { throw new NotImplementedException(); } }";

        // Act
        var messages = _rule.Validate(filePath, fileContent);

        // Assert
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(_rule.Id, messages[0]?.RuleId);
        Assert.AreEqual(Severity.Warning, messages[0]?.Severity);
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
        var messages = _rule.Validate(filePath, fileContent);

        // Assert
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(_rule.Id, messages[0]?.RuleId);
        Assert.AreEqual(Severity.Warning, messages[0]?.Severity);
    }
}