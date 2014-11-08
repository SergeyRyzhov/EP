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
            PlacementGlobal placement;
            Size size;
            Bitmap bitmap;
            GenerateTestDesign(out design, out placement, out size, out bitmap);

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImpl();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save(TestFile("BitmapDrawingTest"));
        }

        [TestMethod]
        public void BitmapDrawingNetTest()
        {
            Design design;
            PlacementGlobal placement;
            Size size;
            Bitmap bitmap;
            GenerateTestDesign(out design, out placement, out size, out bitmap);

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save(TestFile("BitmapDrawingNetTest"));
        }

        [TestMethod]
        public void BitmapDrawingZoomTest()
        {
            Design design;
            PlacementGlobal placement;
            Size size;
            Bitmap bitmap;

            GenerateTestDesign(out design, out placement, out size, out bitmap);

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImpl();
                canvas.Clear(Color.Empty);
                drawer.DrawRect(design, placement, size, canvas, 1, 1, design.field.cellsx - 2, design.field.cellsy - 2);
            }

            bitmap.Save(TestFile("BitmapDrawingZoomTest"));

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImpl();
                canvas.Clear(Color.Empty);
                drawer.Draw(design, placement, size, canvas);
            }
            bitmap.Save(TestFile("BitmapDrawingZoomTestAllArea"));
        }



        private static void GenerateTestDesign(out Design design, out PlacementGlobal placement, out Size size, out Bitmap bitmap)
        {
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
            size = new Size(imageSide, imageSide);
            bitmap = new Bitmap(size.Width, size.Height);
        }

        private static string TestFile(string name)
        {
            return string.Format("..\\..\\{0}.png", name);
        }

        private static void GenerateTestDesign2(out Design design, out PlacementGlobal placement)
        {
            IGenerator generator = new RandomGenerator();

            const int n = 700;
            const int maxx = 50;
            const int maxy = 50;
            const int p = 90;
            const int nets = 20;
            const int maxNetSize = 50;

            const int mx = maxx / 2; //мат.ожидание
            const int my = maxy / 2;

            const double volume = n * mx * my * (100.0 / p);
            int side = Convert.ToInt32(Math.Ceiling(Math.Sqrt(volume)));

            generator.NextDesignWithPlacement(n, nets, maxNetSize, p, maxx, maxy, side, side, out design, out placement);
        }

        [TestMethod]
        public void BitmapDrawingZoomTestComposite()
        {
            Design design;
            PlacementGlobal placement;
            Size size = new Size(2048, 2048);

            GenerateTestDesign2(out design, out placement);

            var w = design.field.cellsx;
            var h = design.field.cellsy;

            var dw = w > 256 ? 256 : w;
            var dh = h > 256 ? 256 : h;

            for (int x = 0; x < w; x += dw)
                {
                    for (int y = 0; y < h; y += dh)
                    {
                        var bitmap = new Bitmap(size.Width, size.Height);
                        using (Graphics canvas = Graphics.FromImage(bitmap))
                        {
                            IDrawer drawer = new DrawerImpl();
                            drawer.DrawRect(
                                design,
                                placement,
                                new Size(size.Width, size.Height),
                                canvas,
                                x,
                                y,
                                dw,
                                dh);
                        }

                        var file = string.Format("ZoomDraw-{0}-{1}.png", x, y);
                        bitmap.Save(file);
                    }
                }    
        }
    }
}