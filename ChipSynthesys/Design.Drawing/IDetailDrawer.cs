using PlaceModel;
using System.Drawing;

namespace ChipSynthesys.Draw
{
    internal interface IDetailDrawer
    {
        void Draw(Design design, PlacementDetail placement, Graphics canvas);
    }
}