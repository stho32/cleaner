namespace cleaner.Domain.Configuration;

public class CleanerConfig
{
    public int MethodLengthMaxSemicolons { get; set; } = 20;
    public int CyclomaticComplexityThreshold { get; set; } = 4;
    public int MaxRowsPerFile { get; set; } = 500;
    public int MaxLinqSteps { get; set; } = 2;
    public int MaxIfStatementDots { get; set; } = 2;
    public int MaxForEachDots { get; set; } = 2;
    public int MaxNestedIfDepth { get; set; } = 2;
    public int MaxFilesPerDirectory { get; set; } = 6;
    public int SqlKeywordThreshold { get; set; } = 3;
}
