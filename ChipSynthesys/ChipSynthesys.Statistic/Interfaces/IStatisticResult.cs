using ChipSynthesys.Statistic.Models;

namespace ChipSynthesys.Statistic.Interfaces
{
    public interface IStatisticResult
    {
        Result<int> PlacedAmount { get; }
        Result<double> ManhattanMetrik { get; }
        Result<double> AreaOfInterserctions { get; }
        Result<int> InterserctionsAmount { get; }
        Result<Interserction[]> Interserctions { get; }
        ComponentsMetrik<double> Distance { get; }
//        ComponentsMetrik<double> DistanceFromNetCenter { get; }
        string ToString();
    }
}