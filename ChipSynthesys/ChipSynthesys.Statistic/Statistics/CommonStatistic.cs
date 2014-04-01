using ChipSynthesys.Statistic.Common;
using ChipSynthesys.Statistic.Interfaces;
using ChipSynthesys.Statistic.Results;
using ChipSynthesys.Statistic.Rows;
using PlaceModel;

namespace ChipSynthesys.Statistic.Statistics
{
    public class CommonStatistic : IStatistic<double, double>
    {
        public static string Name
        {
            get { return StatisticNames.Common; }
        }

        string IStatistic<double, double>.Name
        {
            get { return Name; }
        }

        public void PlacementStatistic(Design design, PlacementGlobal placement, out IStatisticResult<double> result)
        {
            var statisticResult = new StatisticResult();

            statisticResult.Add(new PlacedRow { Design = design, GlobalPlacement = placement });

            result = statisticResult;
        }

        public void PlacementStatistic(Design design, PlacementDetail placement, out IStatisticResult<double> result)
        {
            var statisticResult = new StatisticResult();

            statisticResult.Add(new PlacedRow { Design = design, DetailPlacement = placement });

            result = statisticResult;
        }

        public void DesignStatistic(Design design, out IStatisticResult<double> result)
        {
            var statisticResult = new StatisticResult();
            statisticResult.Add(new ComponentAmountRow { Design = design });

            result = statisticResult;
        }
    }
}