using ChipSynthesys.Common.Classes;
using ChipSynthesys.Statistic.Common;
using ChipSynthesys.Statistic.Interfaces;
using PlaceModel;
using System;
using System.Linq;


namespace ChipSynthesys.Statistic.Rows
{
    class AreaOfIntersectionsRow : ISatisticRow<double>
    {
        public Design Design { get; set; }

        public PlacementDetail DetailPlacement { get; set; }             

        public string Key { get { return StatisticKeys.TotalAreaOfIntersections; } }

        string ISatisticRow<double>.Key
        {
            get { return Key; }
        }

        public double Compute()
        {
            if (DetailPlacement != null)
            {
                return CriterionHelper.AreaOfCrossing(Design, DetailPlacement);
            }
            return 0;
        }
    }
}
