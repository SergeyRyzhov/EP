using System.Drawing;
using ChipSynthesys.Draw;
using PlaceModel;

namespace TestRunner
{
    public class DrawerHelper
    {
        public static void SimpleDraw(Design design, PlacementDetail resultPlacement, Size size, Bitmap bitmap, string fileName)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                canvas.Clear(Color.Empty);
                drawer.DrawRect(design, resultPlacement, size, canvas, 0, 0, design.field.cellsx, design.field.cellsy);
            }

            bitmap.Save(fileName);
        }

        public static void SimpleDraw(Design design, PlacementGlobal resultPlacement, Size size, Bitmap bitmap, string fileName)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                canvas.Clear(Color.Empty);
                drawer.DrawRect(design, resultPlacement, size, canvas, 0, 0, design.field.cellsx, design.field.cellsy);
            }

            bitmap.Save(fileName);
        }
    }
}