using System.Reflection;
using cleaner.Domain.IgnoreComments;
using cleaner.Domain.Rules;

namespace cleaner.Domain;

public static class RuleFactory
{
    public static IRule[] GetRules(HashSet<string> allowedUsings, string fileContent)
    {
        var ruleInstances = CreateRuleInstances(allowedUsings);
        var ignoredRuleIds = CleanerCommentParser.GetIgnoredRuleIds(fileContent);

        // Remove the ignored rules
        ruleInstances.RemoveAll(rule => ignoredRuleIds.Contains(rule.Id));

        return ruleInstances.ToArray();
    }

    private static List<IRule> CreateRuleInstances(HashSet<string> allowedUsings)
    {
        // Dynamically load all rule types
        var ruleTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IRule)) && !t.IsAbstract);

        // Create rule instances
        var ruleInstances = new List<IRule>();
        foreach (var ruleType in ruleTypes)
        {
            var specialHandlingRequired = ruleType == typeof(AllowedUsingsRule);
            if (specialHandlingRequired)
            {
                // ruleInstances.Add(new AllowedUsingsRule(allowedUsings));
                continue;
            }

            var newInstance = (IRule?) Activator.CreateInstance(ruleType);
            if (newInstance != null)
                ruleInstances.Add(newInstance);
        }

        return ruleInstances;
    }
}