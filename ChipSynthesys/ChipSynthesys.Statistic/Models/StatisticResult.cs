using System;

using ChipSynthesys.Statistic.Interfaces;

namespace ChipSynthesys.Statistic.Models
{
    internal class StatisticResult : IStatisticResult
    {
        public StatisticResult(
            Result<int> placedAmount,
            Result<double> manhattanMetric,
            Result<Interserction[]> intersections,
            ComponentsMetrik<double> distance)
        {
            PlacedAmount = placedAmount;
            this.ManhattanMetric = manhattanMetric;
            this.Intersections = intersections;
            Distance = distance;
        }

        public StatisticResult()
        {
        }

        public string Name { get; internal set; }

        public TimeSpan Time { get; internal set; }

        public int ComponentsAmount { get; internal set; }

        public int NetsAmount { get; internal set; }

        public Result<int> PlacedAmount { get; internal set; }

        public Result<double> ManhattanMetric { get; internal set; }

        public Result<double> AreaOfIntersections { get; internal set; }

        public Result<int> IntersectionsAmount { get; internal set; }

        public Result<Interserction[]> Intersections { get; internal set; }

        public ComponentsMetrik<double> Distance { get; internal set; }

        public ChartPair<string, double>[] DistanceChart { get; internal set; }

        public ComponentsMetrik<double> GlobalDistance { get; internal set; }

        public ChartPair<string, double>[] GlobalDistanceChart { get; internal set; }

        public ComponentsMetrik<double> DistanceFromNetCenter { get; internal set; }

        public ChartPair<string, double>[] DistanceFromNetCenterChart { get; internal set; }
    }
}