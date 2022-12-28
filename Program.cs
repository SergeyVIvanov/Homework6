var arr = new string[] { "a", "ccc", "bb" };
Console.WriteLine("Max elem: {0}", arr.GetMax(s => s.Length));

Console.WriteLine();
int count = 0;
var dirGlob = new DirGlob(@"C:\", "*.*", SearchOption.TopDirectoryOnly);
dirGlob.FileFound += DirGlob_FileFound;
dirGlob.Glob();
dirGlob.FileFound -= DirGlob_FileFound;

Console.ReadKey();

void DirGlob_FileFound(object sender, DirGlob.FileFoundEventArgs e)
{
    Console.WriteLine(e.FilePath);

    count++;
    e.Stop = count == 2;
}

public static class EnumerableExtensions
{
    public static T? GetMax<T>(this IEnumerable<T> e, Func<T, float> getParameter) where T : class
    {
        var res = e.Aggregate((MaxItem: (T?)null, MaxValue: float.MinValue), (acc, item) =>
        {
            float value = getParameter(item);
            if (value > acc.MaxValue)
            {
                acc.MaxItem = item;
                acc.MaxValue = value;
            }
            return acc;
        }).MaxItem;

        return res;
    }
}

public class DirGlob
{
    public class FileFoundEventArgs: EventArgs
    {
        public string FilePath { get; }
        public bool Stop { get; set; }

        public FileFoundEventArgs(string filePath)
        {
            FilePath = filePath;
        }
    }

    public delegate void FileFoundEventHandler(object sender, FileFoundEventArgs e);

    public string DirPath { get; }
    public SearchOption SearchOption { get; }
    public string SearchPattern { get; }

    public event FileFoundEventHandler? FileFound;

    public DirGlob(string dirPath, string searchPattern, SearchOption searchOption)
    {
        DirPath = dirPath;
        SearchPattern = searchPattern;
        SearchOption = searchOption;
    }

    public void Glob()
    {
        foreach(string filePath in Directory.EnumerateFiles(DirPath, SearchPattern, SearchOption))
        {
            var e = new FileFoundEventArgs(filePath);
            OnFileFound(e);
            if (e.Stop)
                break;
        }
    }

    private void OnFileFound(FileFoundEventArgs e)
    {
        FileFound?.Invoke(this, e);
    }
}