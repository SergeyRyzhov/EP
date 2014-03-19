using System.Collections.Generic;

namespace ChipSynthesys.Statistic
{
    public interface IStatisticResult <T>
    {
        Dictionary<string, T> Results { get; }

        string ToString();
    }
}