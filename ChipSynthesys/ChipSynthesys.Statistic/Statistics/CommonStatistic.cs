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
        public IStatisticResult Compute(ChipTask task)
        {
            var design = task.Design;
            PlacementGlobal taskPlacement;
            if (task.CurrentPlacement == null)
            {
                taskPlacement = task.GlobalPlacement;
            }
            else
            {
                taskPlacement = new PlacementGlobal(task.Design);
                foreach (var component in task.Design.components)
                {
                    taskPlacement.placed[component] = task.GlobalPlacement.placed[component];
                    taskPlacement.x[component] = task.CurrentPlacement.x[component];
                    taskPlacement.y[component] = task.CurrentPlacement.y[component];
                }
            }

            var statisticResult = new StatisticResult();
            statisticResult.Name = task.Name;
            statisticResult.ComponentsAmount = design.components.Length;
            statisticResult.NetsAmount = design.nets.Length;
            statisticResult.PlacedAmount = new Result<int>(design.components.Count(c => taskPlacement.placed[c]));
            statisticResult.ManhattanMetric = new Result<double>(CriterionHelper.ComputeMetrik(design, taskPlacement));
            statisticResult.IntersectionsAmount = new Result<int>(CriterionHelper.CountOfCrossings(design, taskPlacement));
            statisticResult.AreaOfIntersections = new Result<double>(CriterionHelper.AreaOfCrossing(design, taskPlacement));

            return statisticResult;
        }

        public IStatisticResult Update(IStatisticResult current, ChipTask task, PlacementDetail solution, TimeSpan time)
        {
            var design = task.Design;
            var global = task.GlobalPlacement;
            PlacementGlobal taskPlacement;
            if (task.CurrentPlacement == null)
            {
                taskPlacement = task.GlobalPlacement;
            }
            else
            {
                taskPlacement = new PlacementGlobal(task.Design);
                foreach (var component in task.Design.components)
                {
                    taskPlacement.placed[component] = task.GlobalPlacement.placed[component];
                    taskPlacement.x[component] = task.CurrentPlacement.x[component];
                    taskPlacement.y[component] = task.CurrentPlacement.y[component];
                }
            }

            var statisticResult = current as StatisticResult;
            if (statisticResult == null)
            {
                throw new NotSupportedException();
            }

            statisticResult.Time = time;

            if (statisticResult.PlacedAmount != null)
            {
                statisticResult.PlacedAmount.After = design.components.Count(c => solution.placed[c]);
            }

            if (statisticResult.ManhattanMetric != null)
            {
                statisticResult.ManhattanMetric.After = CriterionHelper.ComputeMetrik(design, solution);
            }

            if (statisticResult.IntersectionsAmount != null)
            {
                statisticResult.IntersectionsAmount.After = CriterionHelper.CountOfCrossings(design, solution);
            }

            if (statisticResult.AreaOfIntersections != null)
            {
                statisticResult.AreaOfIntersections.After = CriterionHelper.AreaOfCrossing(design, solution);
            }

            statisticResult.Distance = new ComponentsMetrik<double>();
            statisticResult.GlobalDistance = new ComponentsMetrik<double>();

            var distance =
                new Func<Component, PlacementGlobal, PlacementDetail, double>(
                    (c, g, d) =>
                    Math.Sqrt((g.x[c] - d.x[c]) * (g.x[c] - d.x[c]) + (g.y[c] - d.y[c]) * (g.y[c] - d.y[c])));
            var distance1 =
                new Func<Component, PlacementGlobal, PlacementGlobal, double>(
                    (c, g, d) =>
                    Math.Sqrt((g.x[c] - d.x[c]) * (g.x[c] - d.x[c]) + (g.y[c] - d.y[c]) * (g.y[c] - d.y[c])));

            foreach (var component in design.components)
            {
                statisticResult.Distance[component] = distance1(component, global, taskPlacement);
                statisticResult.GlobalDistance[component] = distance(component, global, solution);
            }

            var squires = new int[design.components.Length];
            for (int i = 0; i < design.components.Length; i++)
            {
                var component = design.components[i];
                squires[i] = component.sizex * component.sizey;
            }

            statisticResult.DistanceChart =
                squires.Distinct()
                    .OrderBy(s => -s)
                    .Where(s => s > 0)
                    .Select(
                        i =>
                        new ChartPair<string, double>
                            {
                                Abscissa = string.Format("{0} ({1})", i, design.components.Count(c => (c.sizex * c.sizey) == i)),
                                Ordinate =
                                    design.components.Where(co => co.sizex * co.sizey == i)
                                    .Average(c => statisticResult.Distance[c])
                            })
                    .ToArray();

            statisticResult.GlobalDistanceChart =
                squires.Distinct()
                    .OrderBy(s => -s)
                    .Where(s => s > 0)
                    .Select(
                        i =>
                        new ChartPair<string, double>
                            {
                                Abscissa = string.Format("{0} ({1})", i, design.components.Count(c => (c.sizex * c.sizey) == i)),
                                Ordinate =
                                    design.components.Where(co => co.sizex * co.sizey == i)
                                    .Average(c => statisticResult.GlobalDistance[c])
                            })
                    .ToArray();


            return statisticResult;
        }
    }
}