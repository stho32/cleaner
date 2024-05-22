using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules
{
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
            Assert.That(result, Is.Empty);
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
            Assert.That(result.Length, Is.EqualTo(3));

            AssertValidationMessage(result[0], "test.cs", "publicField");
            AssertValidationMessage(result[1], "test.cs", "publicStaticField");
            AssertValidationMessage(result[2], "test.cs", "publicReadonlyField");
        }
    
        private void AssertValidationMessage(ValidationMessage? message, string filePath, string fieldName)
        {
            Assert.That(message?.RuleId, Is.EqualTo("NoPublicFieldsRule"));
            Assert.That(message?.RuleName, Is.EqualTo("No Public Fields Rule"));
            Assert.That(message?.ErrorMessage, Is.EqualTo($"The file '{filePath}' contains a public field: '{fieldName}'. This is not allowed."));
        }
    }
}
