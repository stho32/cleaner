using cleaner.Domain.FileBasedRules.Rules;

namespace cleaner.Domain.FileBasedRules;

public class RuleLister
{
    public void ListAllRules()
    {
        var rules =
            RuleFactory.GetRules(AllowedUsingsRule.GetDefaultAllowedUsings(), "");

        Console.WriteLine("List of existing rules:");

        foreach (var rule in rules)
        {
            Console.WriteLine($"- {rule.Name}");
            Console.WriteLine($"             {rule.LongDescription}");
        }
    }
}