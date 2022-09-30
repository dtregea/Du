/// Daniel Tregea
using System.Diagnostics;
public class Program {
    public static void Main(string[] args)
    {
        var HELP_MESSAGE = string.Join(
            Environment.NewLine,
            "Usage: dotnet run -- [-s] [-p] [-b] <path>",
            "Summarize disk usage of the set of FILES, recursively for directories.",
            "-s  Run in single threaded mode",
            "-p  Run in parallel mode (uses all available processors)",
            "-b  Run in both parallel and single threaded mode.",
            "    Runs parallel followed by sequential mode"
            );

        if (args.Length != 2)
        {
            Console.WriteLine(HELP_MESSAGE);
            return;
        }

        if (!(args[0] == "-s" || args[0] == "-p" || args[0] == "-b"))
        {
            Console.WriteLine(HELP_MESSAGE);
            return;
        }
        
        var directoryInfo = new DirectoryInfo(args[1]);
        if (!directoryInfo.Exists)
        {
            Console.WriteLine("du: cannot access \'" + args[1] + "\': No such file or directory");
            return;
        }

        Console.WriteLine("Directory: " + args[1]);

        var directoryCounter = new DirectoryCounter(directoryInfo);

        switch (args[0])
        {
            case "-s":
                RunSequential(directoryCounter);
                break;
            case "-p":
                RunParallel(directoryCounter);
                break;
            default:
                RunParallel(directoryCounter);
                RunSequential(directoryCounter);
                break;
        }
    }

    private static void RunSequential( DirectoryCounter directoryCounter)
    {
        var sw = new Stopwatch();
        sw.Start();
        directoryCounter.ExecuteSequential();
        sw.Stop();
        Console.WriteLine($"Sequential Calculated in: {sw.Elapsed.TotalSeconds}s");
        DirectoryCounter.PrintCount();
        DirectoryCounter.ResetCount();
    }
    private static void RunParallel( DirectoryCounter directoryCounter)
    {
        var sw = new Stopwatch();
        sw.Start();
        directoryCounter.ExecuteParallel();
        sw.Stop();
        Console.WriteLine($"Parallel Calculated in: {sw.Elapsed.TotalSeconds}s");
        DirectoryCounter.PrintCount();
        DirectoryCounter.ResetCount();
    }
}


