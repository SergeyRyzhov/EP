using PlaceModel;
using System.Collections.Generic;
using System.Linq;

namespace DetailPlacer.Algorithm.CompontsOrderer.Impl
{
    public class CompontsOrderer : ICompontsOrderer
    {
        public override string ToString()
        {
            return "Раскраска графа";
        }

        public void SortComponents(Design design, PlacementGlobal approximate, PlacementDetail result, Component[] unplacedComponents, ref int[] perm)
        {
            var n = unplacedComponents.Count();
            var cols = new List<int>();
            var rowIndex = new List<int>();
            var indx = 0;

            var degMas = new int[n];
            rowIndex.Add(0);

            for (var i = 0; i < n; i++)
            {
                var r1 = unplacedComponents[i];
                var x1 = approximate.x[r1];
                var y1 = approximate.y[r1];
                var deg = 0;

                for (var j = 0; j < n; j++)
                {
                    if (j == i) continue;
                    var r2 = unplacedComponents[j];
                    var x2 = approximate.x[r2];
                    var y2 = approximate.y[r2];
                    if (x1 + r1.sizex <= x2 || x2 + r2.sizex <= x1
                        || y1 + r1.sizey <= y2 || y2 + r2.sizey <= y1) continue;
                    cols.Add(j);
                    indx++;
                    deg++;
                }

                rowIndex.Add(indx);
                degMas[i] = deg;
            }
            rowIndex.Add(cols.Count());

            var colors = new int[n];
            for (var i = 0; i < n; i++) colors[i] = -1;

            var maxDeg = degMas.Max();

            do
            {
                for (var i = 0; i < n; i++)
                {
                    if (degMas[i] != maxDeg) continue;
                    var color = 0;
                    for (var j = rowIndex[i]; j < rowIndex[i + 1]; j++)
                    {
                        if (colors[cols[j]] == color)
                        {
                            color++;
                            j = rowIndex[i];
                        }
                    }
                    colors[i] = color;
                }
                maxDeg--;
            } while (colors.Contains(-1));

            var sortComp = new List<int>();
            for (var i = 0; i <= colors.Max(); i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (colors[j] != i) continue;
                    sortComp.Add(j);
                }
            }

            perm = sortComp.ToArray();
        }
    }
}