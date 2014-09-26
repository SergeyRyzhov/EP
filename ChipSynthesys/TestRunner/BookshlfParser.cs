using System;
using System.IO;
using System.Linq;

using ChipSynthesys.Common.Classes;

using PlaceModel;

namespace TestRunner
{
    public class BookshlfParser
    {
        public ChipTask Parse(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            string auxFile = files.FirstOrDefault(f => f.EndsWith(".aux", StringComparison.InvariantCultureIgnoreCase));
            if (auxFile == null)
            {
                throw new InvalidOperationException("Attempt to parse incorrect example");
            }

            string[] exampleFiles = GetExampleFiles(auxFile);

            string nodesFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".nodes")) ?? "");
            string netsFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".nets")) ?? "");
            string wtsFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".wts")) ?? "");
            string plFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".pl")) ?? "");
            string sclFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".scl")) ?? "");

            int numTerminals;
            int nodesSize;
            Component.Pool components = ParseComponents(nodesFile, out nodesSize, out numTerminals);

            var nets = new Net.Pool();
            string line;
            int numNets;
            int numPins;
            Component[] nodes = components.Extract();
            using (StreamReader netsStream = GetStream(netsFile))
            {
                while ((line = netsStream.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    string[] lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineData.Contains("NumNets"))
                    {
                        numNets = int.Parse(lineData.Last());
                        continue;
                    }

                    if (lineData.Contains("NumPins"))
                    {
                        numPins = int.Parse(lineData.Last());
                        continue;
                    }

                    if (lineData.Contains("NetDegree"))
                    {
                        int size = int.Parse(lineData.Last());
                        var items = new Component[size];
                        for (int i = 0; i < size; i++)
                        {
                            string netLine = netsStream.ReadLine();
                            if (netLine == null)
                            {
                                continue;
                            }

                            string[] netData = netLine.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

                            string nodeName = netData[0];
                            int nodeId = int.Parse(nodeName.Substring(1));
                            int id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                            items[i] = nodes.First(c => c.id.Equals(id));
                        }
                        nets.Add(items);
                    }
                }
            }
            int left = int.MaxValue;
            int right = int.MinValue;

            int top = int.MaxValue;
            int bottom = int.MinValue;

            Design design;
            PlacementGlobal placement;
            using (StreamReader plStream = GetStream(plFile))
            {
                Component[] e = components.Extract();
                while ((line = plStream.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    string[] lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string nodeName = lineData[0];
                    int nodeId = int.Parse(nodeName.Substring(1));
                    int id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                    int x = int.Parse(lineData[1]);
                    int y = int.Parse(lineData[2]);
                    Component component = e.First(c => c.id.Equals(id));

                    left = left > x ? x : left;
                    right = (right < x + component.sizex) ? x + component.sizex : right;
                    top = top > y ? y : top;
                    bottom = (bottom < y + component.sizey) ? y + component.sizey : bottom;
                }
            }
            var field = new Field(left, top, right - left, bottom - top);
            design = new Design(field, components, nets);
            if (design.components.Length != nodesSize)
            {
                throw new InvalidOperationException("Invalid nodes file");
            }

            placement = new PlacementGlobal(design);
            using (StreamReader plStream = GetStream(plFile))
            {
                while ((line = plStream.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    string[] lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string nodeName = lineData[0];
                    int nodeId = int.Parse(nodeName.Substring(1));
                    int id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                    int x = int.Parse(lineData[1]);
                    int y = int.Parse(lineData[2]);
                    Component component = design.components.First(c => c.id.Equals(id));
                    placement.placed[component] = nodeName[0] == 'p'; // || (x == 0 && y == 0);
                    placement.x[component] = x;
                    placement.y[component] = y;
                }
            }

            var result = new ChipTask(design, placement);
            result.Height = 50;
            result.Width = 50;
            return result;
        }


        private static string[] GetExampleFiles(string auxFile)
        {
            StreamReader auxStream = GetStream(auxFile);
            string auxContent = auxStream.ReadToEnd();
            string[] exampleFiles = auxContent.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
            const string AuxHeader = "RowBasedPlacement";
            if (!exampleFiles.Contains(AuxHeader))
            {
                throw new InvalidOperationException("Invalid aux file");
            }

            return exampleFiles.Where(s => !string.Equals(AuxHeader, s)).ToArray();
        }

        private static StreamReader GetStream(string filePath)
        {
            var fs = new FileStream(filePath, FileMode.Open);
            return new StreamReader(fs);
        }

        private static Component.Pool ParseComponents(string nodesFile, out int nodesSize, out int numTerminals)
        {
            var components = new Component.Pool();
            using (StreamReader nodesStream = GetStream(nodesFile))
            {
                string line; // = nodesStream.ReadLine();
                nodesSize = 0;
                numTerminals = 0;
                while ((line = nodesStream.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    string[] lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineData.Contains("NumNodes"))
                    {
                        nodesSize = int.Parse(lineData.Last());
                        continue;
                    }
                    if (lineData.Contains("NumTerminals"))
                    {
                        numTerminals = int.Parse(lineData.Last());
                        continue;
                    }
                    string nodeName = lineData[0];
                    int nodeId = int.Parse(nodeName.Substring(1));
                    int id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                    int sx = int.Parse(lineData[1]);
                    int sy = int.Parse(lineData[2]);
                    components.Add(id, sx, sy);
                    //line = nodesStream.ReadLine();
                }
            }
            return components;
        }

        private Design ParseDesign(
            string nodesFile,
            string netsFile,
            string wtsFile,
            string sclFile,
            out int numTerminals)
        {
            int nodesSize;
            Component.Pool components = ParseComponents(nodesFile, out nodesSize, out numTerminals);

            var nets = new Net.Pool();
            string line;
            int numNets;
            int numPins;
            Component[] nodes = components.Extract();
            StreamReader netsStream = GetStream(netsFile);
            while ((line = netsStream.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                string[] lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (lineData.Contains("NumNets"))
                {
                    numNets = int.Parse(lineData.Last());
                    continue;
                }

                if (lineData.Contains("NumPins"))
                {
                    numPins = int.Parse(lineData.Last());
                    continue;
                }

                if (lineData.Contains("NetDegree"))
                {
                    int size = int.Parse(lineData.Last());
                    var items = new Component[size];
                    for (int i = 0; i < size; i++)
                    {
                        string netLine = netsStream.ReadLine();
                        if (netLine == null)
                        {
                            continue;
                        }

                        string[] netData = netLine.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

                        string nodeName = netData[0];
                        int nodeId = int.Parse(nodeName.Substring(1));
                        int id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                        items[i] = nodes.First(c => c.id.Equals(id));
                    }
                    nets.Add(items);
                }
            }
            var field = new Field(-33, -33, 2400, 2400);
            var design = new Design(field, components, nets);
            if (design.components.Length != nodesSize)
            {
                throw new InvalidOperationException("Invalid nodes file");
            }
            return design;
        }

        private PlacementGlobal ParsePlacement(Design design, string plFile, int numTerminals)
        {
            var placement = new PlacementGlobal(design);
            string line;
            StreamReader plStream = GetStream(plFile);
            while ((line = plStream.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                string[] lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                string nodeName = lineData[0];
                int nodeId = int.Parse(nodeName.Substring(1));
                int id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                int x = int.Parse(lineData[1]);
                int y = int.Parse(lineData[2]);
                Component component = design.components.First(c => c.id.Equals(id));
                placement.placed[component] = nodeName[0] == 'p'; // || (x == 0 && y == 0);
                placement.x[component] = x;
                placement.y[component] = y;
            }

            return placement;
        }
    }
}