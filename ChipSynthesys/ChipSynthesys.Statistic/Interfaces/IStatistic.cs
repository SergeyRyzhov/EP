using PlaceModel;

namespace ChipSynthesys.Statistic.Interfaces
{
    public interface IStatistic<TP, TD>
    {
        string Name { get; }

        void PlacementStatistic(Design design, PlacementGlobal placement, out IStatisticResult<TP> result);

        void PlacementStatistic(Design design, PlacementDetail placement, out IStatisticResult<TP> result);

        void DesignStatistic(Design design, out IStatisticResult<TD> result);
    }
}