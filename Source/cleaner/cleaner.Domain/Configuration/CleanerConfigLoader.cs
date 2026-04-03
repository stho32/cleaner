using System.Text.Json;

namespace cleaner.Domain.Configuration;

public static class CleanerConfigLoader
{
    private const string ConfigFileName = ".cleaner.json";

    public static CleanerConfig Load(string directoryPath)
    {
        var configPath = Path.Combine(directoryPath, ConfigFileName);

        if (!File.Exists(configPath))
            return new CleanerConfig();

        try
        {
            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<CleanerConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new CleanerConfig();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Warning: Could not read {ConfigFileName}: {e.Message}. Using defaults.");
            return new CleanerConfig();
        }
    }
}
