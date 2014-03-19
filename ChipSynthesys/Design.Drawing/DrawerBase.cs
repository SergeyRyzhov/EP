using System.Drawing;
using PlaceModel;

namespace ChipSynthesys.Draw
{
    internal abstract class DrawerBase : IDrawer
    {
        public abstract void Draw(Design design, PlacementGlobal placement, Graphics canvas);

        public abstract void Draw(Design design, PlacementDetail placement, Graphics canvas);
    }
}
