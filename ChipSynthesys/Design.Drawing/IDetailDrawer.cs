using System.Drawing;
using PlaceModel;

namespace ChipSynthesys.Draw
{
    interface IDetailDrawer
    {
        void Draw(Design design, PlacementDetail placement, Graphics canvas);
    }
}