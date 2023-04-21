using cleaner.Domain.Rules;
using cleaner.Domain.Tests.Mocks;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

public class CompositeRuleTests
{
    [Test]
    public void CompositeRule_WithNoRules_ReturnsNoValidationMessages()
    {
        // Arrange
        var compositeRule = new CompositeRule(Enumerable.Empty<IRule>());

        // Act
        var messages = compositeRule.Validate("test.cs", "class Test {}");

        // Assert
        Assert.IsEmpty(messages);
    }

    [Test]
    public void CompositeRule_WithSingleRule_ReturnsValidationMessagesFromRule()
    {
        // Arrange
        var rule = new RuleMock(
            "M001",
            true, 
            new ValidationMessage(Severity.Info,
                "M001",
                "Mock Rule", 
                "This is a mock rule."));
        
        var compositeRule = new CompositeRule(new[] { rule });

        // Act
        var messages = compositeRule.Validate("test.cs", "class Test {}");

        // Assert
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual(rule.Id, messages[0]?.RuleId);
    }

    [Test]
    public void CompositeRule_WithMultipleRules_ReturnsValidationMessagesFromAllRules()
    {
        // Arrange
        var rule1 = new RuleMock(
            "M001",
            true, 
            new ValidationMessage( 
            Severity.Info, 
            "M001",
            "Mock Rule",
            "This is a mock rule."));
        var rule2 = new RuleMock(
            "M002",
            true, 
            new ValidationMessage( 
                Severity.Info, 
                "M002",
                "Mock Rule",
                "This is a mock rule."));
        
        var compositeRule = new CompositeRule(new[] { rule1, rule2 });

        // Act
        var messages = compositeRule.Validate("test.cs", "class Test {}");

        // Assert
        Assert.AreEqual(2, messages.Length);
        Assert.IsTrue(messages.Any(msg => msg?.RuleId == rule1.Id));
        Assert.IsTrue(messages.Any(msg => msg?.RuleId == rule2.Id));
    }
}