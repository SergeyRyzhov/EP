using ChipSynthesys.Common.Classes;
using ChipSynthesys.Statistic.Common;
using ChipSynthesys.Statistic.Interfaces;
using PlaceModel;

namespace ChipSynthesys.Statistic.Rows
{
    class CountOfCrossingsRow : ISatisticRow<double>
    {
        public Design Design { get; set; }

        public PlacementDetail DetailPlacement { get; set; }

        public string Key { get { return StatisticKeys.CountOfIntersections; } }

        string ISatisticRow<double>.Key
        {
            get { return Key; }
        }

        public double Compute()
        {
            if (DetailPlacement != null)
            {
                return (double)CriterionHelper.CountOfCrossings(Design, DetailPlacement);
            }
            return 0;
        }
    }
}
