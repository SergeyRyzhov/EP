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
            var detail = new PlacementDetail(design);
            foreach (var component in design.components)
            {
                detail.placed[component] = placement.placed[component];
                detail.x[component] = (int)placement.x[component];
                detail.y[component] = (int)placement.y[component];
                placement.placed[component] = true;
            }

            statisticResult.Add(new ManhattanMetrikRow { Design = design, GlobalPlacement = placement });
            statisticResult.Add(new AreaOfIntersectionsRow { Design = design, DetailPlacement = detail });
            statisticResult.Add(new CountOfCrossingsRow { Design = design, DetailPlacement = detail });
            foreach (var component in design.components)
            {
                placement.placed[component] = detail.placed[component];
            }
            result = statisticResult;
        }

        public void PlacementStatistic(Design design, PlacementDetail placement, out IStatisticResult<double> result)
        {
            var statisticResult = new StatisticResult();

            statisticResult.Add(new PlacedRow { Design = design, DetailPlacement = placement });
            statisticResult.Add(new ManhattanMetrikRow { Design = design, DetailPlacement = placement });
            statisticResult.Add(new AreaOfIntersectionsRow { Design = design, DetailPlacement = placement });
            statisticResult.Add(new CountOfCrossingsRow { Design = design, DetailPlacement = placement });
           
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