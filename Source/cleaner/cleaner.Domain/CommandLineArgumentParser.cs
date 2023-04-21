using System;
using System.IO;

namespace cleaner.Domain
{
    public class CommandLineArgumentParser
    {
        public string? DirectoryPath { get; private set; }
        public bool IsValid { get; private set; }
        public bool StopOnFirstFileWithProblems { get; private set; }

        public CommandLineArgumentParser(string[] args)
        {
            IsValid = false;
            StopOnFirstFileWithProblems = false;

            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a directory path as a command-line argument.");
                return;
            }

            string directoryPath = args[0];

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"The directory '{directoryPath}' does not exist.");
                return;
            }

            DirectoryPath = directoryPath;
            IsValid = true;

            // Check for optional "stopOnFirstFileWithProblems" or "-s" argument
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].Equals("--stopOnFirstFileWithProblems", StringComparison.OrdinalIgnoreCase)
                    || args[i].Equals("-s", StringComparison.OrdinalIgnoreCase))
                {
                    StopOnFirstFileWithProblems = true;
                }
                else
                {
                    Console.WriteLine($"Unknown argument: {args[i]}");
                    IsValid = false;
                    return;
                }
            }
        }

        public static void PrintUsage()
        {
            Console.WriteLine("Usage: cleaner <directory_path> [--stopOnFirstFileWithProblems | -s]");
            Console.WriteLine("\nArguments:");
            Console.WriteLine("  <directory_path>                 The path of the directory to be processed.");
            Console.WriteLine("  --stopOnFirstFileWithProblems    Optional. If present, the process will stop on the first file with problems.");
            Console.WriteLine("  -s                                Short version of --stopOnFirstFileWithProblems.");
        }
    }
}
