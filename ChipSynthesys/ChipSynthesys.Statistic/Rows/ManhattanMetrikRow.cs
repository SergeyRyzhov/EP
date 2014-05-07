using ChipSynthesys.Common.Classes;
using ChipSynthesys.Statistic.Common;
using ChipSynthesys.Statistic.Interfaces;
using PlaceModel;
using System;
using System.Linq;

namespace ChipSynthesys.Statistic.Rows
{
    internal class ManhattanMetrikRow : ISatisticRow<double>
    {
        public Design Design { get; set; }

        public PlacementDetail DetailPlacement { get; set; }

        public PlacementGlobal GlobalPlacement { get; set; }

        public static string Key { get { return StatisticKeys.ManhattanMetrik; } }

        string ISatisticRow<double>.Key
        {
            get { return Key; }
        }

        public double Compute()
        {
            if (DetailPlacement != null)
            {
                return CriterionHelper.ComputeMetrik(Design, DetailPlacement);
            }
            return CriterionHelper.ComputeMetrik(Design, GlobalPlacement);
        }
    }
}