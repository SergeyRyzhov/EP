using System;
using System.Linq;
using ChipSynthesys.Common.Classes;
using ChipSynthesys.Statistic.Interfaces;
using ChipSynthesys.Statistic.Models;
using PlaceModel;

namespace ChipSynthesys.Statistic.Statistics
{
    public class CommonStatistic
    {
        public IStatisticResult Compute(Design design, PlacementGlobal global, PlacementDetail detail)
        {
            var statisticResult = new StatisticResult();
            statisticResult.ComponentsAmount = design.components.Length;
            statisticResult.NetsAmount = design.nets.Length;
            statisticResult.PlacedAmount = new Result<int>(design.components.Count(c => global.placed[c]));
            statisticResult.ManhattanMetrik = new Result<double>(CriterionHelper.ComputeMetrik(design, global));
            statisticResult.InterserctionsAmount = new Result<int>(CriterionHelper.CountOfCrossings(design, global));
            statisticResult.AreaOfInterserctions = new Result<double>(CriterionHelper.AreaOfCrossing(design, global));
            //statisticResult.Interserctions = new Result<Interserction[]>();
//            sr

            return statisticResult;
        }

        public IStatisticResult Update(IStatisticResult current, Design design, PlacementGlobal global, PlacementDetail detail)
        {
            var statisticResult = current as StatisticResult;
            if (statisticResult == null)
            {
                throw new NotSupportedException();
            }

            if (statisticResult.PlacedAmount != null)
            {
                statisticResult.PlacedAmount.After = design.components.Count(c => detail.placed[c]);
            }

            if (statisticResult.ManhattanMetrik != null)
            {
                statisticResult.ManhattanMetrik.After = CriterionHelper.ComputeMetrik(design, detail);
            }
            if (statisticResult.InterserctionsAmount != null)
            {
                statisticResult.InterserctionsAmount.After = CriterionHelper.CountOfCrossings(design, detail);
            }
            if (statisticResult.AreaOfInterserctions != null)
            {
                statisticResult.AreaOfInterserctions.After = CriterionHelper.AreaOfCrossing(design, detail);
            }

            statisticResult.Distance = new ComponentsMetrik<double>();

            var distance =
                new Func<Component, PlacementGlobal, PlacementDetail, double>(
                    (c, g, d) => Math.Sqrt((g.x[c] - d.x[c])*(g.x[c] - d.x[c]) + (g.y[c] - d.y[c])*(g.y[c] - d.y[c])));
            foreach (var component in design.components)
            {
                statisticResult.Distance[component] = distance(component, global, detail);
                
            }

            var squeres = new int[design.components.Length];
            for (int i = 0; i < design.components.Length; i++)
            {
                var component = design.components[i];
                squeres[i] = component.sizex*component.sizey;
            }


            statisticResult.SquareDistance = squeres.Distinct().OrderBy(s=>s).Where(s => s > 0).Select(i => new ChartPair<int, double>()
            {
                Abscissa = i,
                Ordinate = design.components.Where(co => co.sizex * co.sizey == i).Average(c =>
                {
                    double d = statisticResult.Distance[c];
                    if(d>0) Console.WriteLine(d);
                    return d;
                })
            }).ToArray();
            

            return statisticResult;
        }
    }
}