using System;
using System.IO;
using cleaner.Domain;

namespace cleaner
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var cmdParser = new CommandLineArgumentParser();

            var result = cmdParser.Parse(args);
            if (string.IsNullOrWhiteSpace(result))
                return;
        }
    }
}