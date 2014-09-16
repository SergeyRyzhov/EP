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

        var nodes = ReadNodes(fileModel.NodesFile, fileModel.PlFile);
        model.Nodes = nodes;

        Console.WriteLine("Elements amount {0}",model.Nodes.Length);
        Console.WriteLine("Terminal element amount {0}", model.Nodes.Count(n => n.IsTerminal));
        Console.WriteLine("Simple element amount {0}", model.Nodes.Count(n => !n.IsTerminal));
      }

      return model;
    }

    private Node[] ReadNodes(StreamReader nodesFile, StreamReader plFile)
    {
      var np = 0;
      var na = 0;

      var id = 0;
      var ai = 0;

      var i = 0;
      Node[] result = null;
      do
      {
        string line = nodesFile.ReadLine();
        if (line == null)
        {
          break;
        }

        if (line.StartsWith("UCLA") || line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
        {
          continue;
        }

        line = line.Trim();

        var lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
        if (lineData[0] == "NumNodes")
        {
          na = int.Parse(lineData[1]);
          result = new Node[na];
          continue;
        }

        if (lineData[0] == "NumTerminals")
        {
          np = int.Parse(lineData[1]);
          continue;
        }

        if (lineData[0].StartsWith("p") && result != null)
        {
          id = int.Parse(lineData[0].TrimStart('p'));
          var node = new Node(id) { IsTerminal = true, Sx = int.Parse(lineData[1]), Sy = int.Parse(lineData[2]) };
          result[i++] = node;
          continue;
        }


        if (lineData[0].StartsWith("a") && result != null)
        {
          id = int.Parse(lineData[0].TrimStart('a'));
          var node = new Node(id) { IsTerminal = false, Sx = int.Parse(lineData[1]), Sy = int.Parse(lineData[2]) };
          result[i++] = node;
        }

        /*if (line.StartsWith("RowBasedPlacement"))
        {
          var files = ReadLine(line, ':', ' ');
          Console.WriteLine("Files in benchmark: ");
          files.Each(Console.WriteLine);
        }*/

        //Console.WriteLine(line);
      }
      while (true);
      return result;
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

  public class Node
  {
    public int Id;

    public bool IsTerminal;

    public bool Placed;

    public int X;

    public int Y;

    public int Sx;

    public int Sy;

    public Node(int id)
    {
      this.Id = id;
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
    public Node[] Nodes { get; set; }
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
