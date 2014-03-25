using PlaceModel;
using System.Drawing;

namespace ChipSynthesys.Draw
{
    public interface IGlobalDrawer
    {
        void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas);
    }
}