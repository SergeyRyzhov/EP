namespace ChipSynthesys.Statistic.Interfaces
{
    public interface ISatisticRow<out T>
    {
        string Key { get; }

        T Compute();
    }
}