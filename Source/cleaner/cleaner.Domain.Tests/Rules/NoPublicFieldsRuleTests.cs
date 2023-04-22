using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class NoPublicFieldsRuleTests
{
    private NoPublicFieldsRule _rule = null!;

    [SetUp]
    public void SetUp()
    {
        _rule = new NoPublicFieldsRule();
    }

    [Test]
    public void Validate_FileWithNoPublicFields_ShouldReturnEmpty()
    {
        // Arrange
        var fileContent = @"
            using System;

            namespace TestNamespace
            {
                class TestClass
                {
                    private int _privateField;
                    internal string InternalField;
                    protected float ProtectedField;
                    internal static double StaticInternalField;
                    private static decimal StaticPrivateField;
                }
            }";

        // Act
        var result = _rule.Validate("test.cs", fileContent);

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public void Validate_FileWithPublicFields_ShouldReturnWarning()
    {
        // Arrange
        var fileContent = @"
            using System;

            namespace TestNamespace
            {
                class TestClass
                {
                    public int publicField;
                    public static string publicStaticField;
                    public readonly bool publicReadonlyField;
                    internal string internalField;
                    private float privateField;
                    protected double protectedField;
                    protected static decimal protectedStaticField;
                }
            }";

        // Act
        var result = _rule.Validate("test.cs", fileContent);

        // Assert
        Assert.AreEqual(3, result.Length);

        Assert.AreEqual("NoPublicFieldsRule", result[0]?.RuleId);
        Assert.AreEqual("No Public Fields Rule", result[0]?.RuleName);
        Assert.AreEqual("The file 'test.cs' contains a public field: 'publicField'. This is not allowed.",
            result[0]?.ErrorMessage);

        Assert.AreEqual("NoPublicFieldsRule", result[1]?.RuleId);
        Assert.AreEqual("No Public Fields Rule", result[1]?.RuleName);
        Assert.AreEqual("The file 'test.cs' contains a public field: 'publicStaticField'. This is not allowed.",
            result[1]?.ErrorMessage);

        Assert.AreEqual("NoPublicFieldsRule", result[2]?.RuleId);
        Assert.AreEqual("No Public Fields Rule", result[2]?.RuleName);
        Assert.AreEqual("The file 'test.cs' contains a public field: 'publicReadonlyField'. This is not allowed.",
            result[2]?.ErrorMessage);
    }
}