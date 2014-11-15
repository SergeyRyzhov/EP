using System.Linq;

using ChipSynthesys.Statistic.Common;
using ChipSynthesys.Statistic.Interfaces;

using PlaceModel;

namespace ChipSynthesys.Statistic.Rows
{
    public class PlacedRow : ISatisticRow<double>
    {
        public Design Design { get; set; }

        public PlacementDetail DetailPlacement { get; set; }

        public PlacementGlobal GlobalPlacement { get; set; }

        public static string Key { get { return StatisticKeys.Placed; } }

        string ISatisticRow<double>.Key
        {
            get { return Key; }
        }

        public double Compute()
        {
            if (DetailPlacement != null)
            {
                return Design.components.Count(component => DetailPlacement.placed[component]);
            }

            return Design.components.Count(component => GlobalPlacement.placed[component]);
        }
    }
}