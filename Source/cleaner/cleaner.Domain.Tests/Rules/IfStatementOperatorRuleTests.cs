using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

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
        var result = _ifStatementOperatorRule.Validate(filePath, fileContent);

        // Assert
        Assert.IsEmpty(result!);
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
        var result = _ifStatementOperatorRule.Validate(filePath, fileContent);

        // Assert
        Assert.IsEmpty(result!);
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
        var result = _ifStatementOperatorRule.Validate(filePath, fileContent);

        // Assert
        Assert.AreEqual(1, result!.Length);
        Assert.AreEqual(Severity.Warning, result[0]?.Severity);
        Assert.AreEqual("IfStatementOperatorRule", result[0]?.RuleId);
        Assert.AreEqual("If Statement Operator Rule", result[0]?.RuleName);
        StringAssert.Contains("contains operators, which is not allowed", result[0]?.ErrorMessage);
    }
}