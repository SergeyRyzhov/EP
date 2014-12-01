using ChipSynthesys.Common.Generators;
using ChipSynthesys.Draw;
using DetailPlacer.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaceModel;
using System;
using System.Drawing;

namespace ChipSynthesys.UnitTests.Placement
{
    [TestClass]
    public class PlacerTests
    {
        [TestMethod]
        public void DetailPlacerImplTest()
        {
            Design design;
            PlacementGlobal placement;
            Size size;
            Bitmap bitmap;
            GenerateTestDesign(out design, out placement, out size, out bitmap);
            IDetailPlacer placer = new DetailPlacerImpl();
            const string name = "DetailPlacerImpl Random";

            PlaceAndDraw(bitmap, design, placement, size, name, placer);
        }

        [TestMethod]
        public void DetailPlacerImplOnSquareTest()
        {
            Design design;
            PlacementGlobal placement;
            Size size;
            Bitmap bitmap;
            GenerateTestSquareDesign(out design, out placement, out size, out bitmap);
            IDetailPlacer placer = new DetailPlacerImpl();
            const string name = "DetailPlacerImpl Square";

            PlaceAndDraw(bitmap, design, placement, size, name, placer);
        }

        private static void PlaceAndDraw(Bitmap bitmap, Design design, PlacementGlobal placement, Size size, string name,
            IDetailPlacer placer)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();

                drawer.Draw(design, placement, size, canvas);
            }

            bitmap.Save(TestFile(name + " 0. before"));

            var approximate = new PlacementGlobal(design);

            foreach (var component in design.components)
            {
                approximate.x[component] = placement.x[component];
                approximate.y[component] = placement.y[component];
            }
            PlacementDetail detail;
            placer.Place(design, approximate, out detail);
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                canvas.Clear(Color.FromArgb(255, 255, 255, 255));
                IDrawer drawer = new DrawerImpl();

                drawer.Draw(design, detail, size, canvas);
            }

            bitmap.Save(TestFile(name + " 1. after"));
        }

        private static void GenerateTestDesign(out Design design, out PlacementGlobal placement, out Size size, out Bitmap bitmap)
        {
            IGenerator generator = new RandomGenerator();

            const int n = 150;
            const int maxx = 8;
            const int maxy = 8;
            const int p = 70;

            const int mx = maxx / 2; //мат.ожидание
            const int my = maxy / 2;

            const double volume = n * mx * my * (100.0 / p);
            int side = Convert.ToInt32(Math.Ceiling(Math.Sqrt(volume))) / 2;

            generator.NextDesignWithPlacement(n, 500, 4, p, maxx, maxy, side, side, out design, out placement);

            const int scale = 20; //масштаб здесь, внутри должен быть рассчитан по исходным данным
            int imageSide = side * scale + 2 * scale; //2 для переферии
            size = new Size(imageSide, imageSide);
            bitmap = new Bitmap(size.Width, size.Height);
        }

        private static void GenerateTestSquareDesign(out Design design, out PlacementGlobal placement, out Size size, out Bitmap bitmap)
        {
            IGenerator generator = new SquareGenerator();

            const int n = 150;
            const int maxx = 4;
            const int maxy = 6;
            const int p = 70;

            const int mx = maxx / 2; //мат.ожидание
            const int my = maxy / 2;

            const double volume = n * mx * my * (100.0 / p);
            int side = Convert.ToInt32(Math.Ceiling(Math.Sqrt(volume)));

            generator.NextDesignWithPlacement(n, 50, 4, p, maxx, maxy, side, side, out design, out placement);

            const int scale = 20; //масштаб здесь, внутри должен быть рассчитан по исходным данным
            int imageSide = side * scale + 2 * scale; //2 для переферии
            size = new Size(imageSide, imageSide);
            bitmap = new Bitmap(size.Width, size.Height);
        }

        private static string TestFile(string name)
        {
            return string.Format(@"..\\..\\{0}.png", name);
        }
    }
}