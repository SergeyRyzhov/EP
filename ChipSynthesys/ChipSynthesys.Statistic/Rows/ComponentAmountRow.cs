using ChipSynthesys.Statistic.Common;
using ChipSynthesys.Statistic.Interfaces;
using PlaceModel;

namespace ChipSynthesys.Statistic.Rows
{
    public class ComponentAmountRow : ISatisticRow<double>
    {
        public Design Design { get; set; }

        public static string Key { get { return StatisticKeys.ComponentAmount; } }

        string ISatisticRow<double>.Key
        {
            get { return Key; }
        }

        public double Compute()
        {
            return Design.components.Length;
        }
    }
}