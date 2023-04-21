using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class NoOutAndRefParametersRuleTests
{
    private NoOutAndRefParametersRule _rule = null!;

    [SetUp]
    public void Setup()
    {
        _rule = new NoOutAndRefParametersRule();
    }

    [Test]
    public void Validate_MethodsWithoutOutOrRefParameters_ShouldNotReturnWarning()
    {
        string code = @"
            public class TestClass
            {
                public void Method1(int x) {}
                public void Method2(string s, int i, object o) {}
                public void Method3(int i, string s = null) {}
            }";

        var messages = _rule.Validate("TestClass.cs", code);

        Assert.IsEmpty(messages);
    }

    [Test]
    public void Validate_MethodsWithOutOrRefParameters_ShouldReturnWarning()
    {
        string code = @"
            public class TestClass
            {
                public void Method1(int x, ref string s) {}
                public void Method2(out int i, string s, ref object o) { i = 0; }
                public void Method3(ref int i, out string s = null) {}
            }";

        var messages = _rule.Validate("TestClass.cs", code);

        Assert.IsNotEmpty(messages);
        Assert.AreEqual(5, messages.Length);
        Assert.AreEqual("No Out and Ref Parameters Rule", messages[0]?.RuleName);
        Assert.AreEqual(Severity.Warning, messages[0]?.Severity);
        Assert.IsTrue(messages.All(m => m!.ErrorMessage.Contains("Please avoid using out or ref parameters.")));
    }

    [Test]
    public void Validate_IndexerWithOutOrRefParameters_ShouldReturnWarning()
    {
        string code = @"
            public class TestClass
            {
                public string this[int index, ref string s] { get { return null; } set {} }
                public int this[out int i, string s, ref object o] { get { return 0; } set {} }
            }";

        var messages = _rule.Validate("TestClass.cs", code);

        Assert.IsNotEmpty(messages);
        Assert.AreEqual(3, messages.Length);
        Assert.AreEqual("No Out and Ref Parameters Rule", messages[0]?.RuleName);
        Assert.AreEqual(Severity.Warning, messages[0]?.Severity);
        Assert.IsTrue(messages.All(m => m!.ErrorMessage.Contains("Please avoid using out or ref parameters.")));
    }
}