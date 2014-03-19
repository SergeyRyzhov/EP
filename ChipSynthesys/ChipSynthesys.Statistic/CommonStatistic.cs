using System.Linq;
using PlaceModel;

namespace ChipSynthesys.Statistic
{
    public class CommonStatistic : IStatistic<double, double>
    {
        public void PlacementStatistic(Design design, PlacementDetail placement, out IStatisticResult<double> result)
        {
            var statisticResult = new StatisticResult();

            int placed = design.components.Count(component => placement.placed[component]);

            statisticResult.Add(StatisticKeys.Placed, placed);


            result = statisticResult;
        }

        public void DesignStatistic(Design design, out IStatisticResult<double> result)
        {
            var statisticResult = new StatisticResult();
            statisticResult.Add(StatisticKeys.ComponentAmount, design.components.Length);

            result = statisticResult;
        }
    }
}