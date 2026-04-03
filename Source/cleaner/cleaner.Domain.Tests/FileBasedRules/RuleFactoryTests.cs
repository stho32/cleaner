using cleaner.Domain.Configuration;
using cleaner.Domain.FileBasedRules;
using cleaner.Domain.FileBasedRules.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules;

public class RuleFactoryTests
{
    [Test]
    public void GetRules_DefaultConfig_ReturnsAllRules()
    {
        var rules = RuleFactory.GetRules(
            AllowedUsingsRule.GetDefaultAllowedUsings(), "", new CleanerConfig());

        Assert.That(rules.Length, Is.EqualTo(18));
    }

    [Test]
    public void GetRules_WithIgnoreComment_FiltersRule()
    {
        var fileContent = "// cleaner: ignore MethodLengthRule";
        var rules = RuleFactory.GetRules(
            AllowedUsingsRule.GetDefaultAllowedUsings(), fileContent, new CleanerConfig());

        Assert.That(rules.Any(r => r.Id == "MethodLengthRule"), Is.False);
        Assert.That(rules.Length, Is.EqualTo(17));
    }

    [Test]
    public void GetRules_ConfigPassedToRules()
    {
        var config = new CleanerConfig { MethodLengthMaxSemicolons = 42 };
        var rules = RuleFactory.GetRules(
            AllowedUsingsRule.GetDefaultAllowedUsings(), "", config);

        var methodRule = rules.OfType<MethodLengthRule>().Single();
        Assert.That(methodRule.MaxLength, Is.EqualTo(42));
    }

    [Test]
    public void GetRules_AllRulesHaveIdAndName()
    {
        var rules = RuleFactory.GetRules(
            AllowedUsingsRule.GetDefaultAllowedUsings(), "", new CleanerConfig());

        foreach (var rule in rules)
        {
            Assert.That(rule.Id, Is.Not.Null.And.Not.Empty, $"Rule {rule.GetType().Name} has no Id");
            Assert.That(rule.Name, Is.Not.Null.And.Not.Empty, $"Rule {rule.GetType().Name} has no Name");
        }
    }
}
