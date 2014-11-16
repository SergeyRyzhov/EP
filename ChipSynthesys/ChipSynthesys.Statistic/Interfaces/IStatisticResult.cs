using ChipSynthesys.Statistic.Models;

namespace ChipSynthesys.Statistic.Interfaces
{
    public interface IStatisticResult
    {
        int ComponentsAmount { get; }
        int NetsAmount { get; }
        Result<int> PlacedAmount { get; }
        Result<double> ManhattanMetrik { get; }
        Result<double> AreaOfInterserctions { get; }
        Result<int> InterserctionsAmount { get; }
        Result<Interserction[]> Interserctions { get; }
        ComponentsMetrik<double> Distance { get; }
        ChartPair<int,double>[] SquareDistance { get; }
//        ComponentsMetrik<double> DistanceFromNetCenter { get; }
        string ToString();
    }
}