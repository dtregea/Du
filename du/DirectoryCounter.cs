/// Daniel Tregea
public class DirectoryCounter
{
    private readonly DirectoryInfo directoryInfo;

    private static object ByteLock { get; set; } = new Object();
    private static object FolderLock { get; set; }= new Object();
    private static object FileLock { get; set; } = new Object();

    private static long Folders { get; set; }
    private static long Bytes { get;  set; }
    private static long Files { get; set; }

    /// <summary>
    /// Initialize a DirectoryCounter with a DirectoryInfo
    /// </summary>
    /// <param name="directoryInfo">DirectoryInfo for a directory</param>
    public DirectoryCounter(DirectoryInfo directoryInfo)
    {
        this.directoryInfo = directoryInfo;
    }
    
    private void CountDirectory(Action<DirectoryInfo[]> recurseCallback)
    {
        try
        {
            foreach (var f in directoryInfo.EnumerateFiles("*.*"))
            {
                lock (FileLock)
                {
                    Files++;
                }

                lock (ByteLock)
                {
                    Bytes += f.Length;
                }
            }
            var directories = directoryInfo.GetDirectories();
            lock (FolderLock)
            {
                Folders += directories.Length;
            }
            
            recurseCallback(directories);
        }
        catch (Exception){}
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
                new DirectoryCounter(directory).ExecuteParallel();
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
                new DirectoryCounter(directory).ExecuteSequential();
            }
        });
    }
    
    /// <summary>
    /// Print the results of ExecuteParallel or ExecuteSequential
    /// </summary>
    public static void PrintCount()
    {   
        Console.WriteLine($"{Folders} folders, {Files:n0} files, {Bytes:n0} bytes");
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