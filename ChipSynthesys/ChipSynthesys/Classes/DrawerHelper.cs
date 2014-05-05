using System.Drawing;
using ChipSynthesys.Draw;
using PlaceModel;

namespace ChipSynthesys.Common.Classes
{
    public class DrawerHelper
    {
        public static void SimpleDraw(Design design, PlacementDetail resultPlacement, Size size, Bitmap bitmap, string fileName)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                canvas.Clear(Color.Empty);
                drawer.Draw(design, resultPlacement, size, canvas);
            }
            bitmap.Save(fileName);
        }

        public static void SimpleDraw(Design design, PlacementGlobal resultPlacement, Size size, Bitmap bitmap, string fileName)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                canvas.Clear(Color.Empty);
                drawer.Draw(design, resultPlacement, size, canvas);
            }
            bitmap.Save(fileName);
        }
    }
}