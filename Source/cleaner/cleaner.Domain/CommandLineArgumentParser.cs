namespace cleaner.Domain;

public class CommandLineArgumentParser
{
    public CommandLineArgumentParser()
    {
    }

    public string? Parse(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a directory path as a command-line argument.");
            return null;
        }

        string directoryPath = args[0];

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"The directory '{directoryPath}' does not exist.");
            return null;
        }

        return directoryPath;
    }
}