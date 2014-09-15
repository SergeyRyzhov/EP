using System;
using System.IO;
using System.Linq;

namespace BookshelfParser
{
  public class Program
  {
    static void Main(string[] args)
    {
      if (args.Length > 0)
      {
        var bookshelfFolder = args[0];

        var parser = new BookshelfParser();
        var model = parser.Parse(bookshelfFolder);
        return;
      }

      Console.WriteLine("Incorrect input. First arg: bookshlf folder");
    }
  }

  public class BookshelfParser
  {
    public BooksheftModel Parse()
    {
      var booksheftModel = new BooksheftModel();

      return booksheftModel;
    }

    public BooksheftModel Parse(string bookshelfFolder)
    {
      var model = new BooksheftModel();
      using (var fileModel = ParseFiles(bookshelfFolder))
      {
#if DEBUG
        {
          do
          {
            string line = fileModel.AuxFile.ReadLine();
            if (line == null)
            {
              break;
            }

            if (line.StartsWith("RowBasedPlacement"))
            {
              var files = ReadLine(line, ':', ' ');
              Console.WriteLine("Files in benchmark: ");
              files.Each(Console.WriteLine);
            }

            //Console.WriteLine(line);
          }
          while (true);
        }
#endif
        

      }
      return model;
    }

    private BookshelfFileModel ParseFiles(string bookshelfFolder)
    {
      var bmDir = Directory.GetFiles(bookshelfFolder);

      var auxFile = bmDir.FirstOrDefault(n => n.EndsWith(".aux"));
      var netsFile = bmDir.FirstOrDefault(n => n.EndsWith(".nets"));
      var nodesFile = bmDir.FirstOrDefault(n => n.EndsWith(".nodes"));
      var plFile = bmDir.FirstOrDefault(n => n.EndsWith(".pl"));
      var sclFile = bmDir.FirstOrDefault(n => n.EndsWith(".scl"));
      var wtsFile = bmDir.FirstOrDefault(n => n.EndsWith(".wts"));
      return new BookshelfFileModel(auxFile, netsFile, nodesFile, plFile, sclFile, wtsFile);
    }

    private string[] ReadLine(string line, char bound, char separator)
    {
      return line.Split(new[] { bound }, StringSplitOptions.RemoveEmptyEntries)
        .Skip(1)
        .Take(1)
        .First()
        .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
        .ToArray();
    }
  }

  public class BookshelfFileModel : IDisposable
  {
    public BookshelfFileModel(string auxFile, string netsFile, string nodesFile, string plFile, string sclFile, string wtsFile)
    {
      AuxFile = new StreamReader(File.Open(auxFile, FileMode.Open));
      NetsFile = new StreamReader(File.Open(netsFile, FileMode.Open));
      NodesFile = new StreamReader(File.Open(nodesFile, FileMode.Open));
      PlFile = new StreamReader(File.Open(plFile, FileMode.Open));
      SclFile = new StreamReader(File.Open(sclFile, FileMode.Open));
      WtsFile = new StreamReader(File.Open(wtsFile, FileMode.Open));
    }

    public StreamReader AuxFile { get; private set; }

    public StreamReader NetsFile { get; private set; }

    public StreamReader NodesFile { get; private set; }

    public StreamReader PlFile { get; private set; }

    public StreamReader SclFile { get; private set; }

    public StreamReader WtsFile { get; private set; }

    public void Dispose()
    {
      AuxFile.Dispose();
      NetsFile.Dispose();
      NodesFile.Dispose();
      PlFile.Dispose();
      SclFile.Dispose();
      WtsFile.Dispose();
    }
  }

  public class BooksheftModel
  {
    
  }

  public static class Extension
  {
    public static void Each(this string[] array, Action<object> action)
    {
      foreach (var o in array)
      {
        action(o);
      }
    }

    public static void Each(this object[] array, Action<object> action)
    {
      foreach (var o in array)
      {
        action(o);
      }
    }
  }
}
