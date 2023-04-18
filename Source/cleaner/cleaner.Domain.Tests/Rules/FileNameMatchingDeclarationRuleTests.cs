using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class FileNameMatchingDeclarationRuleTests
{
    private FileNameMatchingDeclarationRule _rule;

    [SetUp]
    public void Setup()
    {
        _rule = new FileNameMatchingDeclarationRule();
    }

    [Test]
    public void Validate_FileNameMatchesDeclaredType_ShouldNotReturnWarning()
    {
        string code = @"
        namespace TestNamespace
        {
            class TestClass
            {
            }
        }";

        var messages = _rule.Validate("TestClass.cs", code);

        Assert.IsEmpty(messages);
    }

    [Test]
    public void Validate_FileNameDoesNotMatchDeclaredType_ShouldReturnWarning()
    {
        string code = @"
        namespace TestNamespace
        {
            class TestClass
            {
            }
        }";

        var messages = _rule.Validate("IncorrectFileName.cs", code);

        Assert.IsNotEmpty(messages);
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual("FileNameMatchingDeclarationRule", messages[0].RuleId);
    }

    [Test]
    public void Validate_NoTypeDeclaredInFile_ShouldNotReturnWarning()
    {
        string code = @"
        // This file intentionally contains no type declarations.";

        var messages = _rule.Validate("EmptyFile.cs", code);

        Assert.IsEmpty(messages);
    }
}