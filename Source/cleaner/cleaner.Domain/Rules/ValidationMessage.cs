namespace cleaner.Domain.Rules;

public class ValidationMessage
{
    public Severity Severity { get; }
    public Guid RuleId { get; }
    public string RuleName { get; }
    public string ErrorMessage { get; }

    public ValidationMessage(Severity severity, Guid ruleId, string ruleName, string errorMessage)
    {
        Severity = severity;
        RuleId = ruleId;
        RuleName = ruleName;
        ErrorMessage = errorMessage;
    }
}