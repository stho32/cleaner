using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class IfStatementDotsRuleTests
{
    private IfStatementDotsRule _rule = null!;

    [SetUp]
    public void SetUp()
    {
        _rule = new IfStatementDotsRule();
    }

    [Test]
    public void IfStatementDotsRule_ValidIfStatements_NoWarnings()
    {
        var code = @"
namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            string hello = ""hello"";
            string world = ""world"";
            if (hello.Equals(world)) { }

            int a = 5;
            int b = 10;
            if (a + b == 15) { }
        }
    }
}";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.IsEmpty(messages);
    }

    [Test]
    public void IfStatementDotsRule_InvalidIfStatements_Warnings()
    {
        var code = @"
namespace TestNamespace
{
    public class TestClass
    {
        public string Property { get; set; }

        public void TestMethod()
        {
            string hello = ""hello"";
            string world = ""world"";
            if (hello.Substring(0, 1).Equals(world.Substring(0, 1))) { }

            TestClass obj = new TestClass { Property = ""test"" };
            if (obj.Property.ToUpper().Contains(""A"")) { }
        }
    }
}";

        var messages = _rule.Validate("TestFile.cs", code);
        Assert.AreEqual(2, messages.Length);

        Assert.True(messages[0].ErrorMessage.Contains(":12 "));
        Assert.True(messages[1].ErrorMessage.Contains(":15 "));
    }
}