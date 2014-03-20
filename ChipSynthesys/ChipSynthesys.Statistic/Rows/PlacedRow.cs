using ChipSynthesys.Statistic.Common;
using ChipSynthesys.Statistic.Interfaces;
using PlaceModel;
using System.Linq;

namespace ChipSynthesys.Statistic.Rows
{
    public class PlacedRow : ISatisticRow<double>
    {
        public Design Design { get; set; }

        public PlacementDetail Placement { get; set; }

        public static string Key { get { return StatisticKeys.Placed; } }

        string ISatisticRow<double>.Key
        {
            get { return Key; }
        }

        public double Compute()
        {
            return Design.components.Count(component => Placement.placed[component]);
        }
    }
}