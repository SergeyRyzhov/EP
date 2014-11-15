using System;

using ChipSynthesys.Statistic.Interfaces;

namespace TestRunner
{
    [Serializable]
    public class PlacementStatisticModel : XmlModel
    {
        public PlacementStatisticModel()
        {
            PlacementStatistic = new SerializableDictionary<string, double>();
        }

        public PlacementStatisticModel(IStatisticResult<double> placementStatistic)
        {
            PlacementStatistic = new SerializableDictionary<string, double>();
            foreach (var d in placementStatistic.Results)
            {
                PlacementStatistic.Add(d.Key, d.Value);
            }
        }

        public SerializableDictionary<string, double> PlacementStatistic { get; set; }
    }
}