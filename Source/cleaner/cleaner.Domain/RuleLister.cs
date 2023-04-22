using cleaner.Domain.Rules;

namespace cleaner.Domain;

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