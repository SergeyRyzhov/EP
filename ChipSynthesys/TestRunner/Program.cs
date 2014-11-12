using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static void GenerateTestData(
            out Design[] design,
            out PlacementGlobal[] approximate,
            out Size[] sizes,
            out Bitmap[] bitmaps)
        {
            const int amount = 1;
            design = new Design[amount];
            approximate = new PlacementGlobal[amount];
            sizes = new Size[amount];
            bitmaps = new Bitmap[amount];
            for (int i = 0; i < design.Length; i++)
            {
                GenerateTestDesign(out design[i], out approximate[i], out sizes[i], out bitmaps[i]);
            }
        }

        private static void GenerateTestDesign(
            out Design design,
            out PlacementGlobal placement,
            out Size size,
            out Bitmap bitmap)
        {
            IGenerator generator = new DenseGenerator();
            const int n = 250; //число компонент
            const int maxx = 7; //размер по x
            const int maxy = 6; //размер по y
            const int p = 70; //процент заполнения
            const int nets = 1000; //число сетей
            const int maxNetSize = 9; //длина цепей
            generator.NextDesignWithPlacement(n, nets, maxNetSize, p, maxx, maxy, 0, 0, out design, out placement);

            const int scale = 20; //масштаб здесь, внутри должен быть рассчитан по исходным данным
            size = new Size(design.field.cellsx * scale, design.field.cellsy * scale);
            bitmap = new Bitmap(size.Width, size.Height);
        }

        private static void LoadFromDirectory(
            string path,
            out Design[] design,
            out PlacementGlobal[] approximate,
            out Size[] sizes,
            out Bitmap[] bitmaps)
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

                    var size = new Size(task.Height * scale, task.Width * scale);
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

        private static void Main(string[] args)
        {
            if (args.Length >= 2 && args[0].ToLower() == "-i")
            {
                MakeFrames(args);

                return;
            }

            if (args.Length == 2 && args[0].ToLower() == "-p")
            {
                ParseBookshelf(args);

                return;
            }

            Design[] design;
            PlacementGlobal[] approximate;
            Size[] sizes;
            Bitmap[] bitmaps;

            string resultDirectory = args.Length > 0 ? args[0] + @"\Tests\" : @"D:\TestResults\";
            Directory.CreateDirectory(resultDirectory);

            if (ReadInput(args, out design, out approximate, out sizes, out bitmaps))
            {
                for (int i = 0; i < design.Length; i++)
                {
                    Design d = design[i];
                    PlacementGlobal a1 = approximate[i];
                    var t = new ChipTask(d, a1) { Height = 30, Width = 30 };

                    t.Save(string.Format("Small {0}.bin", i));
                }
            }

            var statistic = new CommonStatistic();
            int testCount = 0;
            var masType = new Type[] { };

            //пока не используются
            //if (false)
            //{
            //    RunCommonTests(design, statistic, resultDirectory, approximate, sizes, bitmaps, testCount);
            //}

            for (int i = 0; i < design.Length; i++)
            {
                IStatisticResult<double> designStatistics;
                statistic.DesignStatistic(design[i], out designStatistics);
                SaveDesignsInfo(resultDirectory, i + 1, designStatistics);
            }

            for (int i = 0; i < design.Length; i++)
            {
                SaveTestResults(resultDirectory, i + 1, 0, design[i], approximate[i], sizes[i], bitmaps[i]);
            }

            Type[] otherPlacers =
                {
                    typeof(CrossReductPlacer),
                    typeof(CrossComponentVariant2),
                    typeof(CrossCompPlacer),
                    typeof(ForceDirectedDetailPlacer), 
                };

            foreach (Type otherPlacerType in otherPlacers)
            {
                ConstructorInfo info = otherPlacerType.GetConstructor(masType);
                if (info != null)
                {
                    var placer = info.Invoke(null) as IDetailPlacer;
                    if (placer == null)
                    {
                        continue;
                    }


                    for (int i = 0; i < design.Length; i++)
                    {
                        var st = new Stopwatch();
                        st.Start();
                        Design d = design[i];
                        PlacementDetail placeRes;

                        testCount++;
                        SaveTestInfo(resultDirectory, testCount, placer);

                        placer.Place(d, approximate[i], out placeRes);

                        foreach (Component c in d.components)
                        {
                            if (!placeRes.placed[c])
                            {
                                placeRes.x[c] = (int)approximate[i].x[c];
                                placeRes.y[c] = (int)approximate[i].y[c];
                                placeRes.placed[c] = true;
                            }
                        }

                        IStatisticResult<double> placemetStatistics;
                        statistic.PlacementStatistic(d, placeRes, out placemetStatistics);
                        SaveTestResults(
                            resultDirectory,
                            i + 1,
                            testCount,
                            d,
                            placeRes,
                            placemetStatistics,
                            sizes[i],
                            bitmaps[i]);
                        st.Stop();
                        Console.WriteLine(@"Время выполнения {0} - {1}", placer, st.Elapsed);
                    }

                }
            }
        }

        private static void MakeFrames(string[] args)
        {
            string fileName = args[1];
            int index = fileName.LastIndexOf('\\');
            string taskName = fileName.Substring(index + 1);
            ChipTask task = ChipTask.Load(fileName);
            Console.WriteLine(@"Task was drawed by frame");
            int w = task.Design.field.cellsx;
            int h = task.Design.field.cellsy;

            int size;
            if (args.Length < 3 || !int.TryParse(args[2], out size))
            {
                size = 256;
            }

            int dw = w > size ? size : w;
            int dh = h > size ? size : h;

            for (int x = 0; x < w; x += dw)
            {
                for (int y = 0; y < h; y += dh)
                {
                    var bitmap = new Bitmap(dw, dh);
                    using (Graphics canvas = Graphics.FromImage(bitmap))
                    {
                        IDrawer drawer = new DrawerImplNets();
                        drawer.DrawRect(
                            task.Design,
                            task.Approximate,
                            new Size(size, size),
                            canvas,
                            x,
                            y,
                            dw,
                            dh);
                    }

                    string file = string.Format("{0}-{1}-{2}.png", taskName, x, y);
                    bitmap.Save(file);
                }
            }
        }

        private static void ParseBookshelf(string[] args)
        {
            var p = new BookshelfParser();
            ChipTask t = p.Parse(args[1]);
            string fileName = args[1].Split(new[] { '\\' }).Last() + ".bin";
            t.Save(fileName);
            Console.WriteLine(@"Task was parsed. File {0} was created", fileName);
        }

        private static bool ReadInput(
            string[] args,
            out Design[] design,
            out PlacementGlobal[] approximate,
            out Size[] sizes,
            out Bitmap[] bitmaps)
        {
            if (args.Length == 0)
            {
                GenerateTestData(out design, out approximate, out sizes, out bitmaps);
                return true;
            }

            LoadFromDirectory(args[0], out design, out approximate, out sizes, out bitmaps);

            if (design.Length == 0)
            {
                GenerateTestData(out design, out approximate, out sizes, out bitmaps);
                return true;
            }

            return false;
        }

        private static void RunCommonTests(
            Design[] design,
            CommonStatistic statistic,
            string resultDirectory,
            PlacementGlobal[] approximate,
            Size[] sizes,
            Bitmap[] bitmaps,
            int testCount)
        {
            var masType = new Type[0];
            Type typeToReflect = typeof(DetailPlacerBase);
            Assembly a = Assembly.GetAssembly(typeToReflect);
            Type[] existingTypes = a.GetTypes();
            Type[] componentsOrders =
                existingTypes.Where(
                    t =>
                    typeof(ICompontsOrderer).IsAssignableFrom(t) && (!t.IsInterface) && (!t.IsAbstract) && t.IsPublic)
                    .ToArray();
            Type[] positionComparers =
                existingTypes.Where(
                    t =>
                    typeof(IPositionComparer).IsAssignableFrom(t) && (!t.IsInterface) && (!t.IsAbstract) && t.IsPublic)
                    .ToArray();
            Type[] positionSearchers =
                existingTypes.Where(
                    t =>
                    typeof(IPositionSearcher).IsAssignableFrom(t) && (!t.IsInterface) && (!t.IsAbstract) && t.IsPublic)
                    .ToArray();
            Type[] positionSorters =
                existingTypes.Where(
                    t =>
                    typeof(IPositionsSorter).IsAssignableFrom(t) && (!t.IsInterface) && (!t.IsAbstract) && t.IsPublic)
                    .ToArray();

            for (int i = 0; i < design.Length; i++)
            {
                IStatisticResult<double> designStatistics;
                statistic.DesignStatistic(design[i], out designStatistics);
                SaveDesignsInfo(resultDirectory, i + 1, designStatistics);
            }

            for (int i = 0; i < design.Length; i++)
            {
                SaveTestResults(resultDirectory, i + 1, 0, design[i], approximate[i], sizes[i], bitmaps[i]);
            }

            {
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
                                            var posComparer = posComp.Invoke(null) as IPositionComparer;
                                            ConstructorInfo posSort =
                                                posSortType.GetConstructor(new[] { typeof(IPositionComparer) });
                                            if (posSort != null)
                                            {
                                                var posSorter =
                                                    posSort.Invoke(new object[] { posComparer }) as IPositionsSorter;

                                                TestPlacer(
                                                    design,
                                                    statistic,
                                                    resultDirectory,
                                                    approximate,
                                                    sizes,
                                                    bitmaps,
                                                    ++testCount,
                                                    compOrder,
                                                    posSearcher,
                                                    posSorter,
                                                    posComparer);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void TestPlacer(
            Design[] design,
            CommonStatistic statistic,
            string resultDirectory,
            PlacementGlobal[] approximate,
            Size[] sizes,
            Bitmap[] bitmaps,
            int testCount,
            ICompontsOrderer compOrder,
            IPositionSearcher posSearcher,
            IPositionsSorter posSorter,
            IPositionComparer posComparer)
        {
            var placer = new DetailPlacerImpl(compOrder, posSearcher, posSorter);

            SaveTestInfo(resultDirectory, testCount, compOrder, posSearcher, posComparer, posSorter);

            for (int i = 0; i < design.Length; i++)
            {
                Design d = design[i];
                PlacementDetail placeRes;

                var st = new Stopwatch();
                st.Start();

                placer.Place(d, approximate[i], out placeRes);
                st.Stop();
                Console.WriteLine(@"Время выполнения {0} - {1}", placer, st.Elapsed);
                foreach (Component c in d.components)
                {
                    if (!placeRes.placed[c])
                    {
                        placeRes.x[c] = (int)approximate[i].x[c];
                        placeRes.y[c] = (int)approximate[i].y[c];
                        placeRes.placed[c] = true;
                    }
                }

                IStatisticResult<double> placemetStatistics;
                statistic.PlacementStatistic(d, placeRes, out placemetStatistics);
                SaveTestResults(resultDirectory, i + 1, testCount, d, placeRes, placemetStatistics, sizes[i], bitmaps[i]);
            }
        }

        private static void SaveDesignsInfo(string path, int designNum, IStatisticResult<double> designStatistic)
        {
            var s = new DesignStatisticModel(designStatistic);
            s.Save(string.Format("{0}Design {1} Statistics.xml", path, designNum));
        }

        private static void SaveTestInfo(
            string path,
            int testNum,
            ICompontsOrderer compOrder,
            IPositionSearcher posComparer,
            IPositionComparer posSearcher,
            IPositionsSorter posSorter)
        {
            var s = new HeuristicsModel(compOrder, posComparer, posSearcher, posSorter);
            s.Save(string.Format("{0}Heuristics {1}.xml", path, testNum));
        }

        private static void SaveTestInfo(string path, int testNum, IDetailPlacer placer)
        {
            var s = new HeuristicsModel(placer);
            s.Save(string.Format("{0}Heuristics {1}.xml", path, testNum));
        }

        private static void SaveTestResults(
            string path,
            int designNum,
            int testNum,
            Design design,
            PlacementDetail resultPlacement,
            IStatisticResult<double> placementStatistic,
            Size size,
            Bitmap bitmap)
        {
            DrawerHelper.SimpleDraw(
                design,
                resultPlacement,
                size,
                bitmap,
                string.Format("{0}Result for design {2} on exp {1}.png", path, testNum, designNum));

            var s = new PlacementStatisticModel(placementStatistic);
            s.Save(string.Format("{0}PlacementStatistics on design {2} in {1} test.xml", path, testNum, designNum));

            var global = new PlacementGlobal(design);
            foreach (Component c in design.components)
            {
                global.x[c] = resultPlacement.x[c];
                global.y[c] = resultPlacement.y[c];
                global.placed[c] = resultPlacement.placed[c];
            }

            var t = new ChipTask(design, global) { Height = 30, Width = 30 };

            t.Save(string.Format("{0}PlacementResult on design {2} in {1} test.bin", path, testNum, designNum));
        }

        private static void SaveTestResults(
            string path,
            int designNum,
            int testNum,
            Design design,
            PlacementGlobal resultPlacement,
            Size size,
            Bitmap bitmap)
        {
            DrawerHelper.SimpleDraw(
                design,
                resultPlacement,
                size,
                bitmap,
                string.Format("{0}Result for design {2} on exp {1}.png", path, testNum, designNum));

            var statistic = new CommonStatistic();
            IStatisticResult<double> placemetStatistics;

            statistic.PlacementStatistic(design, resultPlacement, out placemetStatistics);
            var s = new PlacementStatisticModel(placemetStatistics);
            s.Save(string.Format("{0}PlacementStatistics on design {2} in {1} test.xml", path, testNum, designNum));
        }
    }
}