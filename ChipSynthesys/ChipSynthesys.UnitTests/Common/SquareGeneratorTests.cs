using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChipSynthesys.Common.Generators;
using ChipSynthesys.Draw;
using PlaceModel;
using System.Drawing;

namespace ChipSynthesys.UnitTests.Common
{
    [TestClass]
    public class SquareGeneratorTests
    {
        [TestMethod]
        public void PlacementTest()
        {
            IGenerator generator = new SquareGenerator();
            
            const int n = 25;
            const int maxx = 3;
            const int maxy = 3;
            const int p = 70;

            const int mx = maxx / 2; //мат.ожидание
            const int my = maxy / 2;

            const double volume = n * mx * my * (100.0 / p);
            int side = Convert.ToInt32(Math.Ceiling(Math.Sqrt(volume)));
            side = Math.Max(n * maxx, n * maxy);
           
            Design design;
            PlacementGlobal placement;

            generator.NextDesignWithPlacement(n, 20, 5, p, maxx, maxy, side, side, out design, out placement);

            const int scale = 20; //масштаб здесь, внутри должен быть рассчитан по исходным данным
            int imageSide = side * scale + 2 * scale; //2 для переферии
            var size = new Size(imageSide, imageSide);
            var bitmap = new Bitmap(size.Width, size.Height);
            foreach (var item in design.components)
            {
                placement.placed[item] = false;
            }
            foreach (var item in design.components)
            {
                var name = string.Format("component {0}.png", item.id);
                placement.placed[item] = true;
                DrawDisign(design, placement, size, bitmap, name);
                placement.placed[item] = false;
            }
            foreach (var item in design.components)
            {
                placement.placed[item] = true;
            }
            foreach (var net in design.nets)
            {
                var s = new System.Text.StringBuilder();
                s.Append(net.id);
                s.Append(": ");

                foreach (var component in net.items)
                {
                    s.Append(component.id);
                    s.Append(", ");
                }
                Console.WriteLine(s.ToString());
            }
            DrawDisign(design, placement, size, bitmap, "Test_square_gen.png");
            DrawDisignNet(design, placement,size,bitmap, "Test.png");
        }

        private void DrawDisign(Design design, PlacementGlobal placement, Size size, Bitmap bitmap, string fileName)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImpl();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save(fileName);
        }

        private void DrawDisignNet(Design design, PlacementGlobal placement, Size size, Bitmap bitmap, string fileName)
        {

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save(fileName);
        }

    }
}
