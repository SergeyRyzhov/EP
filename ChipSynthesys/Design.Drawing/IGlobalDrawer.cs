using System.Drawing;
using PlaceModel;

namespace ChipSynthesys.Draw
{
    interface IGlobalDrawer
    {
        void Draw(Design design, PlacementGlobal placement, Graphics canvas);
    }
}