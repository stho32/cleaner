namespace cleaner.Domain.Rules;

public record ValidationMessage(string RuleId, string RuleName, string ErrorMessage);