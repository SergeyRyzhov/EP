using ChipSynthesys.Common.Generators;
using ChipSynthesys.Draw;
using DetailPlacer.Algorithm;
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
            Size size;
            Bitmap bitmap;
            GenerateTestDesign(out design, out placement, out size, out bitmap);

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImpl();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save("..\\..\\test.png");
        }

        [TestMethod]
        public void BitmapDrawingNetTest()
        {
            Design design;
            PlacementDetail placement;
            Size size;
            Bitmap bitmap;
            GenerateTestDesign(out design, out placement, out size, out bitmap);

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save("..\\..\\NetTest.png");
        }

        [TestMethod]
        public void PlacerTest()
        {
            Design design;
            PlacementDetail placement;
            Size size;
            Bitmap bitmap;
            GenerateTestDesign(out design, out placement, out size, out bitmap);

            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImpl();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save("0. before.png");

            IDetailPlacer placer = new DetailPlacerImpl();
            var gp = new PlacementGlobal(design);

            foreach (var component in design.components)
            {
                gp.x[component] = placement.x[component];
                gp.y[component] = placement.y[component];
            }

            placer.Place(design, gp, out placement);
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                canvas.Clear(Color.FromArgb(255,255,255,255));
                IDrawer drawer = new DrawerImpl();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save("1. after.png");
        }


        private void GenerateTestDesign(out Design design, out PlacementDetail placement, out Size size, out Bitmap bitmap)
        {
            IGenerator generator = new RandomGenerator();

            const int n = 15;
            const int maxx = 4;
            const int maxy = 4;
            const int p = 70;

            const int mx = maxx/2; //мат.ожидание
            const int my = maxy/2;

            const double volume = n*mx*my*(100.0/p);
            int side = Convert.ToInt32(Math.Ceiling(Math.Sqrt(volume)));

            generator.NextDesignWithPlacement(n, 15, 4, p, maxx, maxy, side, side, out design, out placement);

            const int scale = 20; //масштаб здесь, внутри должен быть рассчитан по исходным данным
            int imageSide = side*scale + 2*scale; //2 для переферии
            size = new Size(imageSide, imageSide);
            bitmap = new Bitmap(size.Width, size.Height);
        }
    }
}