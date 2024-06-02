namespace cleaner.Domain;

public class StatisticsCollector : IStatisticsCollector
{
    private int _filesWithProblems = 0;
    private int _foldersWithProblems = 0;
    private int _filesScanned = 0;
    private int _foldersScanned = 0;

    public void FoundFileWithProblems()
    {
        _filesWithProblems++;
    }

    public void ScanningFile()
    {
        _filesScanned++;
    }

    public void ScanningFolder()
    {
        _foldersScanned++;
    }

    public void FoundFolderWithProblems()
    {
        _foldersWithProblems++;
    }

    public Statistic GetStatistics()
    {
        return new Statistic(_filesWithProblems, _foldersWithProblems, _filesScanned, _foldersScanned);
    }
}