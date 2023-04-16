namespace cleaner.Domain.Rules;

public record ValidationMessage(Severity Severity, string RuleId, string RuleName, string ErrorMessage);