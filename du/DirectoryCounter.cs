

public class DirectoryCounter
{
    private String Directory;

    private static object ByteLock { get; set; } = new Object();
    private static object FolderLock { get; set; }= new Object();
    private static object FileLock { get; set; } = new Object();

    private static long Folders { get; set; }

    private static long Bytes { get;  set; }

    private static long Files { get; set; }

    /// <summary>
    /// Initialize a DirectoryCounter for a directory
    /// </summary>
    /// <param name="directory">The path to the directory</param>
    public DirectoryCounter(string directory)
    {
        this.Directory = directory;
        
    }

    private void CountDirectory(Action<string[]> recurseCallback)
    {
        try
        {
            var dirFiles = System.IO.Directory.GetFiles(Directory);
            lock (FileLock)
            {
                Files += dirFiles.Length;
            }
            
            foreach (var t in dirFiles)
            {
                var f = new FileInfo(t);
                lock (ByteLock)
                {
                    Bytes += f.Length;
                }
                
            }
            
        }
        catch (UnauthorizedAccessException){}

        try
        {
            var directories = System.IO.Directory.GetDirectories(Directory);
            lock (FolderLock)
            {
                Folders += directories.Length;
            }

            if (directories.Length == 0)
            {
                return;
            }

            recurseCallback(directories);
        }
        catch (UnauthorizedAccessException) {}
    }
    
    /// <summary>
    /// Count Folder, Byte, and File size in the given directory and subdirectories in parallel
    /// </summary>
    public void ExecuteParallel()
    {
        CountDirectory((directories) =>
        {
            Parallel.ForEach(directories, directory =>
            {
                new DirectoryCounter(Directory + "/" + new FileInfo(directory).Name).ExecuteParallel();
            });
        });

    }
    
    /// <summary>
    /// Count Folder, Byte, and File size in the given directory and subdirectories in sequence
    /// </summary>
    public void ExecuteSequential()
    {

        CountDirectory((directories) =>
        {
            foreach (var directory in directories)
            {
                new DirectoryCounter(Directory + "/" + new FileInfo(directory).Name).ExecuteSequential();
            }
        });
    }
    
    /// <summary>
    /// Print the results of ExecuteParallel or ExecuteSequential
    /// </summary>
    public static void PrintCount()
    {   
        Console.WriteLine($"{Folders} folders, {DirectoryCounter.Files:n0} files, {DirectoryCounter.Bytes:n0} bytes");
    }
    
    /// <summary>
    /// Reset the results of ExecuteParallel or ExecuteSequential
    /// </summary>
    public static void ResetCount()
    {
        Folders = 0;
        Files = 0;
        Bytes = 0;
    }
}