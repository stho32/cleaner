namespace cleaner.Domain.Rules;

public class CompositeRule : IRule
{
    public string Id => GetType().Name;
    public string Name => "Composite Rule";
    public string ShortDescription => "A composite rule that contains a collection of rules.";
    public string LongDescription => "This rule is a composite that holds a collection of sub-rules. It executes the sub-rules one after another and returns all the validation messages from the sub-rules.";

    private readonly List<IRule> _rules;

    public CompositeRule(IEnumerable<IRule> rules)
    {
        _rules = rules.ToList();
    }

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        foreach (var rule in _rules)
        {
            var ruleMessages = rule.Validate(filePath, fileContent);
            messages.AddRange(ruleMessages!);
        }

        return messages.ToArray();
    }
}