using ChipSynthesys.Statistic.Interfaces;

namespace ChipSynthesys.Statistic.Models
{
    internal class StatisticResult : IStatisticResult
    {
        public StatisticResult(Result<int> placedAmount, Result<double> manhattanMetrik,
            Result<Interserction[]> interserctions, ComponentsMetrik<double> distance)
        {
            PlacedAmount = placedAmount;
            ManhattanMetrik = manhattanMetrik;
            Interserctions = interserctions;
            Distance = distance;
        }

        public StatisticResult()
        {
        }

        public int ComponentsAmount { get; internal set; }
        public int NetsAmount { get; internal set; }
        public Result<int> PlacedAmount { get; internal set; }
        public Result<double> ManhattanMetrik { get; internal set; }

        public Result<double> AreaOfInterserctions
        { get; internal set; }

        public Result<int> InterserctionsAmount
        { get; internal set; }

        public Result<Interserction[]> Interserctions { get; internal set; }
        public ComponentsMetrik<double> Distance { get; internal set; }
    }
}