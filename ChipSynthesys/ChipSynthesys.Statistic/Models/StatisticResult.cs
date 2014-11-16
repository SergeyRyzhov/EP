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

        public Result<int> PlacedAmount { get; internal set; }
        public Result<double> ManhattanMetrik { get; internal set; }

        public Result<double> AreaOfInterserctions
        {
            get
            {
                var res = new Result<double>();
                return res;
            }
        }

        public Result<int> InterserctionsAmount
        {
            get { var res = new Result<int>();
                return res;
            }
        }

        public Result<Interserction[]> Interserctions { get; internal set; }
        public ComponentsMetrik<double> Distance { get; internal set; }
    }
}