using System;
using System.Drawing;
using System.IO;
using ChipSynthesys.Common.Generators;
using ChipSynthesys.Statistic.Statistics;
using ChipSynthesys.Statistic.Interfaces;
using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer;
using PlaceModel;
using System.Reflection;
using System.Linq;
using DetailPlacer.Algorithm;
using ChipSynthesys.Draw;
using System.Drawing;

namespace TestRunner
{
    public class Program
    {
        private static void SaveTestResults(string path, int designNum,int testNum, Design design, PlacementDetail resultPlacement, IStatisticResult<double> placementStatistic, Size size, Bitmap bitmap)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                drawer.Draw(design, resultPlacement, size, canvas);
            }

            bitmap.Save(path + testNum + " " + designNum + ".png");

            
            using (StreamWriter sw = File.CreateText(path + "placememtStatistics " + testNum + " " + designNum + ".txt"))
            {
                sw.WriteLine(placementStatistic.ToString());
            }
        }
        
        private static void SaveDesignsInfo(string path, int designNum, IStatisticResult<double> designStatistic)
        {
            using (StreamWriter sw = File.CreateText(path + "designStatistics "  + designNum + ".txt"))
            {
                sw.WriteLine(designStatistic.ToString());
            }
        }
    
       
        private static void SaveTestInfo(string path, int testNum, Type compOrder, Type posComparer, Type posSearcher, Type posSorter)
        {
            using (StreamWriter sw = File.CreateText(path + "testInfo " + testNum + ".txt"))
            {                
               sw.WriteLine(compOrder.ToString());
               sw.WriteLine(posComparer.ToString());
               sw.WriteLine(posSearcher.ToString());
               sw.WriteLine(posSorter.ToString());
            }
        }
        
        static void Main(string[] args)
        {
            Design[] design;
            PlacementGlobal[] approximate;
            Size[] sizes;
            Bitmap[] bitmaps;
           
            string resultDerectory = @"D:\TestResults\";//args[?]
            Directory.CreateDirectory(resultDerectory);
            
            ReadInput(args, out design, out approximate, out sizes, out bitmaps);  

            Type typeToReflect = typeof(DetailPlacer.Algorithm.DetailPlacerBase);
            var a = Assembly.GetAssembly(typeToReflect);
            Type[] existingTypes = a.GetTypes();           
            var componentsOrders = existingTypes.Where(t =>
                (typeof(ICompontsOrderer).IsAssignableFrom(t)) && (!t.IsInterface));
            var positionComparers = existingTypes.Where(t=>
                (typeof(IPositionComparer).IsAssignableFrom(t))&&(!t.IsInterface));
            var positionSearchers = existingTypes.Where(t =>
                (typeof(IPositionSearcher).IsAssignableFrom(t)) && (!t.IsInterface));
            var positionSorters = existingTypes.Where(t =>
                (typeof(IPositionsSorter).IsAssignableFrom(t)) && (!t.IsInterface));
     
            componentsOrders = componentsOrders.ToArray();
            positionComparers = positionComparers.ToArray();
            positionSearchers = positionSearchers.ToArray();
            positionSorters = positionSorters.ToArray();
            
            var masType = new Type[] { };
            var statistic = new CommonStatistic();
            IStatisticResult<double> designStatistics;
            IStatisticResult<double> placemetStatistics;
            var testCount = 0;

            for (var i = 0; i < design.Length; i++)
            {
                statistic.DesignStatistic(design[i], out designStatistics);
                SaveDesignsInfo(resultDerectory, i + 1, designStatistics);
            }

            foreach (var comOrderType in componentsOrders)
            {               
                var compOrder = comOrderType.GetConstructor(masType).Invoke(null) as ICompontsOrderer;

                foreach (var posSerchType in positionSearchers)
                {
                    var posSearcher = new PositionSearcher(10);//posSerchType.GetConstructor(masType).Invoke(null) as IPositionSearcher;

                    foreach (var posSortType in positionSorters)
                    {
                        foreach (var posCompType in positionComparers)
                        {
                            var posComparer = posCompType.GetConstructor(masType).Invoke
                                (null) as IPositionComparer;
                            var posSorter = posSortType.GetConstructor(new Type[] { typeof(IPositionComparer) }).Invoke
                                (new Object[]{posComparer}) as IPositionsSorter;
                            var placer = new DetailPlacerImpl(compOrder, posSearcher, posSorter);
                            testCount++;
                            SaveTestInfo(resultDerectory, testCount, comOrderType, posCompType, posSerchType, posSortType);
                            
                            for (var i = 0; i < design.Length; i++)
                            {
                                var d = design[i];
                                var placeRes = new PlacementDetail(d);
                                placer.Place(d,approximate[i],out placeRes);
                                statistic.PlacementStatistic(d, placeRes, out placemetStatistics);
                                SaveTestResults(resultDerectory, i + 1, testCount, d, placeRes, placemetStatistics, sizes[i], bitmaps[i]);
                            }
                        }
                    }
                }
            }

            Console.ReadLine();           

        }

        private static void ReadInput(string[] args, out Design[] design, out PlacementGlobal[] approximate, out Size[] sizes, out Bitmap[] bitmaps)
        {
            if (args.Length == 0)
            {
                GenerateTestData(out design, out approximate, out sizes, out bitmaps);
            }
            else
            {
                LoadFromDirectory(args[1], out design, out approximate, out sizes, out bitmaps);
            }
        }
        /// <summary>
        /// Загружает массивы исходных данных. Инициализирует размеры и 
        /// картинки для дальнейших тестов
        /// </summary>
        private static void LoadFromDirectory(string path, out Design[] design, out PlacementGlobal[] approximate, out Size[] sizes, out Bitmap[] bitmaps)
        {
            //Скорее всего будет либо два файла для схемы и для размещения. Загрузив дин нужно будет подтянуть другой

            var files = Directory.GetFiles(path);
            foreach (string fileName in files)
            {
                
            }

            throw new NotImplementedException();
        }

        private static void GenerateTestData(out Design[] design, out PlacementGlobal[] approximate, out Size[] sizes, out Bitmap[] bitmaps)
        {
            design = new Design[1];
            approximate = new PlacementGlobal[1];
            sizes = new Size[1];
            bitmaps = new Bitmap[1];
            
            GenerateTestDesign(out design[0], out approximate[0], out sizes[0], out bitmaps[0]);
        }

        private static void GenerateTestDesign(out Design design, out PlacementGlobal placement, out Size size, out Bitmap bitmap)
        {
            IGenerator generator = new RandomGenerator();

            const int n = 10;
            const int maxx = 8;
            const int maxy = 8;
            const int p = 70;

            const int mx = maxx / 2; //мат.ожидание
            const int my = maxy / 2;

            const double volume = n * mx * my * (100.0 / p);
            int side = Convert.ToInt32(Math.Ceiling(Math.Sqrt(volume)));

            generator.NextDesignWithPlacement(n, 500, 4, p, maxx, maxy, side, side, out design, out placement);

            const int scale = 20; //масштаб здесь, внутри должен быть рассчитан по исходным данным
            int imageSide = side * scale + 2 * scale; //2 для переферии
            size = new Size(imageSide, imageSide);
            bitmap = new Bitmap(size.Width, size.Height);
        }
    }
}
