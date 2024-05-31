namespace cleaner.Domain.FileBasedRules.Rules;

public record ValidationMessage(string RuleId, string RuleName, string ErrorMessage);