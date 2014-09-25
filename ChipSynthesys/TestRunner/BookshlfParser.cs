using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChipSynthesys.Common.Classes;

using PlaceModel;

namespace TestRunner
{
    public class BookshlfParser
    {
        ChipTask Parse(string directory)
        {
            var files = Directory.GetFiles(directory);
            var auxFile = files.FirstOrDefault(f => f.EndsWith(".aux", StringComparison.InvariantCultureIgnoreCase));
            if (auxFile == null)
            {
                throw new InvalidOperationException("Attempt to parse incorrect example");
            }

            var exampleFiles = GetExampleFiles(auxFile);

            var nodesFile = exampleFiles.FirstOrDefault(s => s.EndsWith(".nodes"));
            var netsFile = exampleFiles.FirstOrDefault(s => s.EndsWith(".nets"));
            var wtsFile = exampleFiles.FirstOrDefault(s => s.EndsWith(".wts"));
            var plFile = exampleFiles.FirstOrDefault(s => s.EndsWith(".pl"));
            var sclFile = exampleFiles.FirstOrDefault(s => s.EndsWith(".scl"));

            Design design = ParseDesign(nodesFile, netsFile, wtsFile, sclFile);
            PlacementGlobal placement = ParsePlacement(design, plFile);

            var result = new ChipTask(design, placement);
            return result;
        }

        private PlacementGlobal ParsePlacement(Design design, string plFile)
        {
            PlacementGlobal placement = new PlacementGlobal(design);

            return placement;
        }

        private Design ParseDesign(string nodesFile, string netsFile, string wtsFile, string sclFile)
        {
            var components = new Component.Pool();
            var nodesStream = GetStream(nodesFile);
            string line = nodesStream.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("#") || line.StartsWith("UCLA") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var lineData = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

                line = nodesStream.ReadLine();
            }
            var nets = new Net.Pool();

            var field = new Field(0, 0, 1, 1);
            Design design = new Design(field, components, nets);
            return design;

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
