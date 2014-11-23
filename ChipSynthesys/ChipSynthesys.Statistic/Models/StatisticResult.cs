﻿using ChipSynthesys.Statistic.Interfaces;

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

        public int ComponentsAmount { get; internal set; }

        public int NetsAmount { get; internal set; }

        public Result<int> PlacedAmount { get; internal set; }

        public Result<double> ManhattanMetric { get; internal set; }

        public Result<double> AreaOfIntersections { get; internal set; }

        public Result<int> IntersectionsAmount { get; internal set; }

        public Result<Interserction[]> Intersections { get; internal set; }

        public ComponentsMetrik<double> Distance { get; internal set; }

        public ChartPair<int, double>[] DistanceChart { get; internal set; }

        public ComponentsMetrik<double> DistanceFromNetCenter { get; internal set; }

        public ChartPair<int, double>[] DistanceFromNetCenterChart { get; internal set; }
    }
}