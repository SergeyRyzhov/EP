using System.Drawing;
using System.Drawing.Imaging;

using ChipSynthesys.Common.Classes;
using ChipSynthesys.Draw;

namespace TestRunner
{
    public static class DrawerHelper
    {
        public static void SimpleDraw(this ChipTask task, string name)
        {
            Size adjustSize = task.Design.AdjustSize();
            DrawGlobal(task, adjustSize, name);
            if (task.CurrentPlacement != null)
            {
                DrawDetail(task, adjustSize, name);
            }
        }

        private static void DrawDetail(ChipTask task, Size size, string name)
        {
            var bitmap = new Bitmap(size.Height, size.Width);
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                canvas.Clear(Color.Empty);
                drawer.Draw(task.Design, task.CurrentPlacement, size, canvas);
            }

            bitmap.Save(string.Format(@"{0}-detail{1}", name, ".png"), ImageFormat.Png);
        }

        private static void DrawGlobal(ChipTask task, Size size, string name)
        {
            var bitmap = new Bitmap(size.Height, size.Width);
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                canvas.Clear(Color.Empty);
                drawer.Draw(task.Design, task.GlobalPlacement, size, canvas);
            }

            bitmap.Save(string.Format(@"{0}-global{1}", name, ".png"), ImageFormat.Png);
        }
    }
}