using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

public class MethodLengthRuleTests
{
    private IRule _methodLengthRule = null!;

    [SetUp]
    public void Setup()
    {
        _methodLengthRule = new MethodLengthRule();
    }

    [Test]
    public void Validate_NoMethods_ShouldReturnNoMessages()
    {
        // Arrange
        string filePath = "test.cs";
        string fileContent = "public class TestClass { }";

        // Act
        var result = _methodLengthRule.Validate(filePath, fileContent);

        // Assert
        Assert.IsEmpty(result!);
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
        var result = _methodLengthRule.Validate(filePath, fileContent);

        // Assert
        Assert.IsEmpty(result!);
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
        var result = _methodLengthRule.Validate(filePath, fileContent);

        // Assert
        Assert.AreEqual(1, result?.Length);
        Assert.AreEqual("MethodLengthRule", result![0]?.RuleId);
        Assert.AreEqual("Method Length Rule", result[0]?.RuleName);
        StringAssert.Contains("more than the allowed limit of 10", result[0]?.ErrorMessage);
    }
}