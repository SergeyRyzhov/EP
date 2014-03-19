using PlaceModel;

namespace ChipSynthesys.Statistic
{
    public interface IStatistic <TP, TD>
    {
        void PlacementStatistic(Design design, PlacementDetail placement, out IStatisticResult<TP> result);

        void DesignStatistic(Design design, out IStatisticResult<TD> result);
    }
}