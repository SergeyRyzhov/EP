using PlaceModel;
using System.Drawing;

namespace ChipSynthesys.Draw
{
    public interface IDetailDrawer
    {
        void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas);
    }
}