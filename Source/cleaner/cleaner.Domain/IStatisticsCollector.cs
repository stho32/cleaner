namespace cleaner.Domain;

public interface IStatisticsCollector
{
    void FoundFileWithProblems();
    void ScanningFile();
    void ScanningFolder();
    void FoundFolderWithProblems();
}