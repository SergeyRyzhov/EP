using PlaceModel;
using System.Drawing;

namespace ChipSynthesys.Draw
{
    public abstract class DrawerBase : IDrawer
    {
        public abstract void Draw(Design design, PlacementGlobal placement, Graphics canvas);

        public abstract void Draw(Design design, PlacementDetail placement, Graphics canvas);
    }
}