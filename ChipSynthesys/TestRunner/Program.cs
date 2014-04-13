using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using ChipSynthesys.Common.Classes;
using ChipSynthesys.Common.Generators;
using ChipSynthesys.Draw;
using ChipSynthesys.Statistic.Interfaces;
using ChipSynthesys.Statistic.Statistics;
using DetailPlacer.Algorithm;
using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer;
using PlaceModel;

namespace TestRunner
{
    public class Program
    {
        private static void SaveTestResults(string path, int designNum, int testNum, Design design,
            PlacementDetail resultPlacement, IStatisticResult<double> placementStatistic, Size size, Bitmap bitmap)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                canvas.Clear(Color.Empty);
                drawer.Draw(design, resultPlacement, size, canvas);
            }

            bitmap.Save(string.Format("{0}Result for design {2} on exp {1}.png", path, testNum, designNum));

            using (
                StreamWriter sw =
                    File.CreateText(string.Format("{0}PlacememtStatistics on design {2} in {1} test.txt", path, testNum,
                        designNum))
                )
            {
                sw.WriteLine(placementStatistic.ToString());
            }
        }

        private static void SaveTestResults(string path, int designNum, int testNum, Design design,
            PlacementGlobal resultPlacement, Size size, Bitmap bitmap)
        {
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImpl();
                canvas.Clear(Color.Empty);
                drawer.Draw(design, resultPlacement, size, canvas);
            }

            bitmap.Save(string.Format("{0}Result for design {2} on exp {1}.png", path, testNum, designNum));
        }

        private static void SaveDesignsInfo(string path, int designNum, IStatisticResult<double> designStatistic)
        {
            using (StreamWriter sw = File.CreateText(string.Format("{0}Design {1} Statistics.txt", path, designNum)))
            {
                sw.WriteLine(designStatistic.ToString());
            }
        }

        private static void SaveTestInfo(string path, int testNum, object compOrder, object posComparer,
            object posSearcher,
            object posSorter)
        {
            using (StreamWriter sw = File.CreateText(string.Format("{0}Heuristics {1}.txt", path, testNum)))
            {
                sw.WriteLine(compOrder.ToString());
                sw.WriteLine(posComparer.ToString());
                sw.WriteLine(posSearcher.ToString());
                sw.WriteLine(posSorter.ToString());
            }
        }

        private static void Main(string[] args)
        {
            Design[] design;
            PlacementGlobal[] approximate;
            Size[] sizes;
            Bitmap[] bitmaps;

            //todo создаём эту папку в папке на которую натравили либо если указан (то параметр номер х) лучше добавить файл конфигурации этой консольки
            string resultDerectory = args.Length > 0 ? args[0] + @"\Tests\" : @"D:\TestResults\"; //args[?]
            Directory.CreateDirectory(resultDerectory);

            if (ReadInput(args, out design, out approximate, out sizes, out bitmaps))
            {
                for (int i = 0; i < design.Length; i++)
                {
                    Design d = design[i];
                    PlacementGlobal a1 = approximate[i];
                    var t = new ChipTask(d, a1)
                    {
                        Height = 30,
                        Width = 30
                    };

                    t.Save(string.Format("Small {0}.bin", i));
                }
            }

            Type typeToReflect = typeof (DetailPlacerBase);
            Assembly a = Assembly.GetAssembly(typeToReflect);
            Type[] existingTypes = a.GetTypes();
            Type[] componentsOrders = existingTypes.Where(t =>
                (typeof (ICompontsOrderer).IsAssignableFrom(t)) && (!t.IsInterface)).ToArray();
            Type[] positionComparers = existingTypes.Where(t =>
                (typeof (IPositionComparer).IsAssignableFrom(t)) && (!t.IsInterface)).ToArray();
            Type[] positionSearchers = existingTypes.Where(t =>
                (typeof (IPositionSearcher).IsAssignableFrom(t)) && (!t.IsInterface)).ToArray();
            Type[] positionSorters = existingTypes.Where(t =>
                (typeof (IPositionsSorter).IsAssignableFrom(t)) && (!t.IsInterface)).ToArray();

            var masType = new Type[] {};
            var statistic = new CommonStatistic();
            IStatisticResult<double> designStatistics;
            IStatisticResult<double> placemetStatistics;
            int testCount = 0;

            for (int i = 0; i < design.Length; i++)
            {
                statistic.DesignStatistic(design[i], out designStatistics);
                SaveDesignsInfo(resultDerectory, i + 1, designStatistics);
            }

            for (int i = 0; i < design.Length; i++)
            {
                SaveTestResults(resultDerectory, i + 1, 0, design[i], approximate[i], sizes[i], bitmaps[i]);
            }


            foreach (Type comOrderType in componentsOrders)
            {
                ConstructorInfo comOrder = comOrderType.GetConstructor(masType);
                if (comOrder != null)
                {
                    var compOrder = comOrder.Invoke(null) as ICompontsOrderer;

                    foreach (Type posSerchType in positionSearchers)
                    {
                        ConstructorInfo posSerch = posSerchType.GetConstructor(masType);
                        if (posSerch != null)
                        {
                            var posSearcher = posSerch.Invoke(null) as IPositionSearcher;

                            foreach (Type posSortType in positionSorters)
                            {
                                foreach (Type posCompType in positionComparers)
                                {
                                    ConstructorInfo posComp = posCompType.GetConstructor(masType);
                                    if (posComp != null)
                                    {
                                        var posComparer = posComp.Invoke
                                            (null) as IPositionComparer;
                                        ConstructorInfo posSort =
                                            posSortType.GetConstructor(new[] {typeof (IPositionComparer)});
                                        if (posSort != null)
                                        {
                                            var posSorter = posSort.Invoke
                                                (new Object[] {posComparer}) as IPositionsSorter;
                                            var placer = new DetailPlacerImpl(compOrder, posSearcher, posSorter);
                                            testCount++;
                                            SaveTestInfo(resultDerectory, testCount, compOrder, posSearcher,
                                                posComparer, posSorter);

                                            for (int i = 0; i < design.Length; i++)
                                            {
                                                Design d = design[i];
                                                PlacementDetail placeRes;

                                                //todo исправить другие части чтобы всё рисовали здесь 
                                                //формирую пустое приближённое решение
                                                var tempAppr = approximate[i];
                                                foreach (var c in d.components)
                                                {
                                                    tempAppr.placed[c] = false;
                                                }

                                                //placer.Place(d, approximate[i], out placeRes);
                                                placer.Place(d, tempAppr, out placeRes);

                                                foreach (var c in d.components)
                                                {
                                                    tempAppr.placed[c] = true;
                                                }

                                                statistic.PlacementStatistic(d, placeRes, out placemetStatistics);
                                                SaveTestResults(resultDerectory, i + 1, testCount, d, placeRes,
                                                    placemetStatistics, sizes[i], bitmaps[i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Console.ReadLine();
        }

        private static bool ReadInput(string[] args, out Design[] design, out PlacementGlobal[] approximate,
            out Size[] sizes, out Bitmap[] bitmaps)
        {
            if (args.Length == 0)
            {
                GenerateTestData(out design, out approximate, out sizes, out bitmaps);
                return true;
            }
            else
            {
                LoadFromDirectory(args[0], out design, out approximate, out sizes, out bitmaps);
            }

            if (design.Length == 0)
            {
                GenerateTestData(out design, out approximate, out sizes, out bitmaps);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Загружает массивы исходных данных. Инициализирует размеры и
        ///     картинки для дальнейших тестов
        /// </summary>
        private static void LoadFromDirectory(string path, out Design[] design, out PlacementGlobal[] approximate,
            out Size[] sizes, out Bitmap[] bitmaps)
        {
            string[] files = Directory.GetFiles(path);
            const int scale = 20;

            var designList = new List<Design>();
            var solutionList = new List<PlacementGlobal>();
            var sizeList = new List<Size>();
            var bitmapList = new List<Bitmap>();
            foreach (string fileName in files)
            {
                string extension = Path.GetExtension(fileName);
                if (extension != null && extension.Equals(".bin", StringComparison.InvariantCultureIgnoreCase))
                {
                    ChipTask task = ChipTask.Load(fileName);
                    designList.Add(task.Design);
                    solutionList.Add(task.Approximate);

                    var size = new Size(task.Height*scale, task.Width*scale);
                    var bitmap = new Bitmap(size.Width, size.Height);

                    sizeList.Add(size);
                    bitmapList.Add(bitmap);
                }
            }

            design = designList.ToArray();
            approximate = solutionList.ToArray();
            sizes = sizeList.ToArray();
            bitmaps = bitmapList.ToArray();
        }

        private static void GenerateTestData(out Design[] design, out PlacementGlobal[] approximate, out Size[] sizes,
            out Bitmap[] bitmaps)
        {
            design = new Design[3];
            approximate = new PlacementGlobal[3];
            sizes = new Size[3];
            bitmaps = new Bitmap[3];
            for (int i = 0; i < design.Length; i++)
            {
                GenerateTestDesign(out design[i], out approximate[i], out sizes[i], out bitmaps[i]);
            }
        }

        private static void GenerateTestDesign(out Design design, out PlacementGlobal placement, out Size size,
            out Bitmap bitmap)
        {
            IGenerator generator = new RandomGenerator();

            const int n = 150;
            const int maxx = 16;
            const int maxy = 16;
            const int p = 70;

            const int mx = maxx/2; //мат.ожидание
            const int my = maxy/2;

            const double volume = n*mx*my*(100.0/p);
            int side = Convert.ToInt32(Math.Ceiling(Math.Sqrt(volume)))/3;

            generator.NextDesignWithPlacement(n, 500, 4, p, maxx, maxy, side, side, out design, out placement);

            const int scale = 20; //масштаб здесь, внутри должен быть рассчитан по исходным данным
            int imageSide = side*scale + 2*scale; //2 для переферии
            size = new Size(imageSide, imageSide);
            bitmap = new Bitmap(size.Width, size.Height);
        }
    }
}