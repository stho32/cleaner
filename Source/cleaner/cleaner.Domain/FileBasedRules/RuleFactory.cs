using cleaner.Domain.Configuration;
using cleaner.Domain.FileBasedRules.IgnoreComments;
using cleaner.Domain.FileBasedRules.Rules;
using cleaner.Domain.FileBasedRules.Rules.NestedIfStatementsRuleValidation;

namespace cleaner.Domain.FileBasedRules;

public static class RuleFactory
{
    public static IRule[] GetRules(HashSet<string> allowedUsings, string fileContent, CleanerConfig config)
    {
        var ruleInstances = CreateRuleInstances(allowedUsings, config);
        var ignoredRuleIds = CleanerCommentParser.GetIgnoredRuleIds(fileContent);

        ruleInstances.RemoveAll(rule => ignoredRuleIds.Contains(rule.Id));

        return ruleInstances.ToArray();
    }

    private static List<IRule> CreateRuleInstances(HashSet<string> allowedUsings, CleanerConfig config)
    {
        return new List<IRule>
        {
            // new AllowedUsingsRule(allowedUsings),
            new CyclomaticComplexityRule(config.CyclomaticComplexityThreshold),
            new FileNameMatchingDeclarationRule(),
            new ForEachDataSourceRule(config.MaxForEachDots),
            new IfStatementDotsRule(config.MaxIfStatementDots),
            new IfStatementOperatorRule(),
            new LinqExpressionLengthRule(config.MaxLinqSteps),
            new MethodLengthRule(config.MethodLengthMaxSemicolons),
            new NestedIfStatementsRule(config.MaxNestedIfDepth),
            new NoConfigurationManagerRule(),
            new NoOutAndRefParametersRule(),
            new NoPublicFieldsRule(),
            new NoPublicGenericListPropertiesRule(),
            new NotImplementedExceptionRule(),
            new PublicPropertiesPrivateSettersRule(),
            new RepositoryInheritanceRule(),
            new RowLimitRule(config.MaxRowsPerFile),
            new SingleDeclarationRule(),
            new SqlInNonRepositoryRule(config.SqlKeywordThreshold)
        };
    }
}