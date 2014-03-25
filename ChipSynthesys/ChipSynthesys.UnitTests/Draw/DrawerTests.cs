using ChipSynthesys.Common.Generators;
using ChipSynthesys.Draw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaceModel;
using System;
using System.Drawing;

namespace ChipSynthesys.UnitTests.Draw
{
    [TestClass]
    public class DrawerTests
    {
        [TestMethod]
        public void BitmapDrawingTest()
        {
            Design design;
            PlacementDetail placement;
            IGenerator generator = new RandomGenerator();

            const int n = 15;
            const int maxx = 4;
            const int maxy = 4;
            const int p = 70;

            const int mx = maxx / 2; //мат.ожидание
            const int my = maxy / 2;

            const double volume = n * mx * my * (100.0 / p);
            int side = Convert.ToInt32(Math.Ceiling(Math.Sqrt(volume)));

            generator.NextDesignWithPlacement(n, 15, 4, p, maxx, maxy, side, side, out design, out placement);

            const int scale = 20; //масштаб здесь, внутри должен быть рассчитан по исходным данным
            int imageSide = side * scale + 2 * scale; //2 для переферии
            var size = new Size(imageSide, imageSide);
            var bitmap = new Bitmap(size.Width, size.Height);

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImpl();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save("test.png");
        }
    }
}