using PlaceModel;
using System.Drawing;

namespace ChipSynthesys.Draw
{
    public interface IDetailDrawer
    {
        void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas);

        void DrawRect(Design design, PlacementDetail placement, Size size, Graphics canvas, int x, int y, int width, int height);
    }
}