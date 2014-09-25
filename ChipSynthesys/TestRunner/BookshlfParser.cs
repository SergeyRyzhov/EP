using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using ChipSynthesys.Common.Classes;

using PlaceModel;

namespace TestRunner
{
    public class BookshlfParser
    {
        public ChipTask Parse(string directory)
        {
            var files = Directory.GetFiles(directory);
            var auxFile = files.FirstOrDefault(f => f.EndsWith(".aux", StringComparison.InvariantCultureIgnoreCase));
            if (auxFile == null)
            {
                throw new InvalidOperationException("Attempt to parse incorrect example");
            }

            var exampleFiles = GetExampleFiles(auxFile);

            var nodesFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".nodes")) ?? "");
            var netsFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".nets")) ?? "");
            var wtsFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".wts")) ?? "");
            var plFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".pl")) ?? "");
            var sclFile = Path.Combine(directory, exampleFiles.FirstOrDefault(s => s.EndsWith(".scl")) ?? "");

            int numTerminals;
            Design design = ParseDesign(nodesFile, netsFile, wtsFile, sclFile, out numTerminals);
            PlacementGlobal placement = this.ParsePlacement(design, plFile, numTerminals);

            var result = new ChipTask(design, placement);
            result.Height = 50;
            result.Width = 50;
            return result;
        }

        private PlacementGlobal ParsePlacement(Design design, string plFile, int numTerminals)
        {
            PlacementGlobal placement = new PlacementGlobal(design);
            string line;
            var plStream = GetStream(plFile);
            while ((line = plStream.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                var nodeName = lineData[0];
                var nodeId = int.Parse(nodeName.Substring(1));
                var id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                var x = int.Parse(lineData[1]);
                var y = int.Parse(lineData[2]);
                var component = design.components.First(c => c.id.Equals(id));
                placement.placed[component] = nodeName[0] == 'p';// || (x == 0 && y == 0);
                placement.x[component] = x;
                placement.y[component] = y;
            }

            return placement;
        }

        private Design ParseDesign(string nodesFile, string netsFile, string wtsFile, string sclFile, out int numTerminals)
        {
            int nodesSize;
            var components = ParseComponents(nodesFile, out nodesSize, out numTerminals);

            var nets = new Net.Pool();
            string line;
            int numNets;
            int numPins;
            Component[] nodes = components.Extract();
            var netsStream = GetStream(netsFile);
            while ((line = netsStream.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
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
                    var size = int.Parse(lineData.Last());
                    var items = new Component[size];
                    for (int i = 0; i < size; i++)
                    {
                        var netLine = netsStream.ReadLine();
                        if (netLine == null)
                        {
                            continue;
                        }

                        var netData = netLine.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

                        var nodeName = netData[0];
                        var nodeId = int.Parse(nodeName.Substring(1));
                        var id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                        items[i] = nodes.First(c => c.id.Equals(id));
                    }
                    nets.Add(items);
                    continue;
                }


            }
            var field = new Field(-33, -33, 2400, 2400);
            Design design = new Design(field, components, nets);
            if (design.components.Length != nodesSize)
            {
                throw new InvalidOperationException("Invalid nodes file");
            }
            return design;

        }

        private static Component.Pool ParseComponents(string nodesFile, out int nodesSize, out int numTerminals)
        {
            var components = new Component.Pool();
            var nodesStream = GetStream(nodesFile);
            string line; // = nodesStream.ReadLine();
            nodesSize = 0;
            numTerminals = 0;
            while ((line = nodesStream.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
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
                var nodeName = lineData[0];
                var nodeId = int.Parse(nodeName.Substring(1));
                var id = nodeName[0] == 'p' ? nodeId - 1 : nodeId + numTerminals; //hack
                var sx = int.Parse(lineData[1]);
                var sy = int.Parse(lineData[2]);
                components.Add(id, sx, sy);
                //line = nodesStream.ReadLine();
            }
            return components;
        }

        private static string[] GetExampleFiles(string auxFile)
        {
            var auxStream = GetStream(auxFile);
            var auxContent = auxStream.ReadToEnd();
            var exampleFiles = auxContent.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
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
    }


}
