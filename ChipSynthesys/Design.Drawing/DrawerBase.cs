using PlaceModel;
using System.Drawing;

namespace ChipSynthesys.Draw
{
    public abstract class DrawerBase : IDrawer
    {
        public abstract void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas);

        public abstract void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas);

        public abstract void DrawRect(Design design, PlacementGlobal placement, Size size, Graphics canvas, int x, int y, int width, int height);

        public abstract void DrawRect(Design design, PlacementDetail placement, Size size, Graphics canvas, int x, int y, int width, int height);
    }
}