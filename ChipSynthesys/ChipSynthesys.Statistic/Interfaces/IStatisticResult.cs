using ChipSynthesys.Statistic.Models;

namespace ChipSynthesys.Statistic.Interfaces
{
    public interface IStatisticResult
    {
        int ComponentsAmount { get; }

        int NetsAmount { get; }

        Result<int> PlacedAmount { get; }

        Result<double> ManhattanMetric { get; }

        Result<double> AreaOfIntersections { get; }

        Result<int> IntersectionsAmount { get; }

        Result<Interserction[]> Intersections { get; }

        ComponentsMetrik<double> Distance { get; }

        ChartPair<int, double>[] DistanceChart { get; }

        ComponentsMetrik<double> DistanceFromNetCenter { get; }

        ChartPair<int, double>[] DistanceFromNetCenterChart { get; }

        string ToString();
    }
}