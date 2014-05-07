using System.Collections.Generic;

namespace ChipSynthesys.Statistic.Interfaces
{
    public interface IStatisticResult<T>
    {
        Dictionary<string, T> Results { get; }

        string ToString();
    }
}