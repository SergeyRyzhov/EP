using PlaceModel;
using System.Drawing;

namespace ChipSynthesys.Draw
{
    internal interface IGlobalDrawer
    {
        void Draw(Design design, PlacementGlobal placement, Graphics canvas);
    }
}