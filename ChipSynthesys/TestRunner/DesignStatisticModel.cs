using System;

using ChipSynthesys.Statistic.Interfaces;

namespace TestRunner
{
    [Serializable]
    public class DesignStatisticModel : XmlModel
    {
        public DesignStatisticModel()
        {
            this.DesignStatistic = new SerializableDictionary<string, double>();
        }

        public DesignStatisticModel(IStatisticResult<double> designStatistic)
        {
            this.DesignStatistic = new SerializableDictionary<string, double>();
            foreach (var d in designStatistic.Results)
            {
                this.DesignStatistic.Add(d.Key, d.Value);
            }
        }

        public SerializableDictionary<string, double> DesignStatistic { get; set; }
    }
}