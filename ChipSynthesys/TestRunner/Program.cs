using System.Globalization;

using ChipSynthesys.Common.Classes;
using ChipSynthesys.Common.Generators;
using ChipSynthesys.Draw;
using ChipSynthesys.Statistic;
using ChipSynthesys.Statistic.Statistics;
using DetailPlacer.Algorithm;
using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.CompontsOrderer.Impl;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.Impl;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;

using PlaceModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

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

            if (args.Length >= 2 && args[0].ToLower() == "-n")
            {
                MakeFrame(args);

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
            //RunCommonTests(design, statistic, resultDirectory, approximate, sizes, bitmaps, ref testCount);
            
            Type[] otherPlacers =
                {
                    //typeof(CombinePlacer),
                    //typeof(CrossReductPlacer),
                    typeof(CrossComponentVariant2),
                    //typeof(CrossCompPlacer),
                    //typeof(ForceDirectedDetailPlacer),
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

                        string imageBefore = Path.Combine(resultDirectory, string.Format("TestData {0}.png", testCount));
                        DrawerHelper.SimpleDraw(
                            design[i],
                            approximate[i],
                            sizes[i],
                            bitmaps[i], imageBefore);

                        placer.Place(d, approximate[i], out placeRes);

                        var statisticResult = statistic.Compute(d, approximate[i], placeRes);
                        statisticResult = statistic.Update(statisticResult, d, approximate[i], placeRes);

                        string fileName = Path.Combine(resultDirectory, string.Format("TestResult {0}.xlsx", testCount));
                        StatisticImporter.SaveToFile(fileName, statisticResult, i.ToString(CultureInfo.InvariantCulture), placer.ToString());

                        string imageAfter = Path.Combine(resultDirectory, string.Format("TestResult {0}.png", testCount));
                        DrawerHelper.SimpleDraw(
                            design[i],
                            placeRes,
                            sizes[i],
                            bitmaps[i], imageAfter);


                        var a = new PlacementGlobal(d);

                        foreach (var c in d.components)
                        {
                            a.placed[c] = placeRes.placed[c];
                            a.x[c] = placeRes.x[c];
                            a.y[c] = placeRes.y[c];

                        }
                        var t = new ChipTask(d, a) { Height = 50/*sizes[i].Height*/, Width = 50/*sizes[i].Width*/ };

                        t.Save(Path.Combine(resultDirectory, string.Format("TestData {0}.bin", testCount)));

                        st.Stop();
                        Console.WriteLine(@"Time for {0} - {1}", placer, st.Elapsed);
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

        private static void MakeFrame(string[] args)
        {
            string fileName = args[1];
            int index = fileName.LastIndexOf('\\');
            string taskName = fileName.Substring(index + 1);
            ChipTask task = ChipTask.Load(fileName);
            Console.WriteLine(@"Task was drawed");
            int w = task.Design.field.cellsx + 100;
            int h = task.Design.field.cellsy + 100;

            var bitmap = new Bitmap(w, h);
            using (Graphics canvas = Graphics.FromImage(bitmap))
            {
                IDrawer drawer = new DrawerImplNets();
                drawer.Draw(
                    task.Design,
                    task.Approximate,
                    new Size(w, h),
                    canvas);
            }

            string file = string.Format("{0}.png", taskName);
            bitmap.Save(file);
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
            ref int testCount)
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
                                                ref testCount,
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

        private static void TestPlacer(
            Design[] design,
            CommonStatistic statistic,
            string resultDirectory,
            PlacementGlobal[] approximate,
            Size[] sizes,
            Bitmap[] bitmaps,
            ref int testCount,
            ICompontsOrderer compOrder,
            IPositionSearcher posSearcher,
            IPositionsSorter posSorter,
            IPositionComparer posComparer)
        {
            var placer = new DetailPlacerImpl(compOrder, posSearcher, posSorter);

            for (int i = 0; i < design.Length; i++)
            {
                var st = new Stopwatch();
                st.Start();
                Design d = design[i];
                PlacementDetail placeRes;
                testCount ++;
                string imageBefore = Path.Combine(resultDirectory, string.Format("TestData {0}.png", testCount));
                DrawerHelper.SimpleDraw(
                    design[i],
                    approximate[i],
                    sizes[i],
                    bitmaps[i], imageBefore);

                placer.Place(d, approximate[i], out placeRes);

                var statisticResult = statistic.Compute(d, approximate[i], placeRes);
                statisticResult = statistic.Update(statisticResult, d, approximate[i], placeRes);

                string fileName = Path.Combine(resultDirectory, string.Format("TestResult {0}.xlsx", testCount));
                StatisticImporter.SaveToFile(fileName, statisticResult, i.ToString(CultureInfo.InvariantCulture), placer.ToString());

                string imageAfter = Path.Combine(resultDirectory, string.Format("TestResult {0}.png", testCount));
                DrawerHelper.SimpleDraw(
                    design[i],
                    placeRes,
                    sizes[i],
                    bitmaps[i], imageAfter);


                var a = new PlacementGlobal(d);

                foreach (var c in d.components)
                {
                    a.placed[c] = placeRes.placed[c];
                    a.x[c] = placeRes.x[c];
                    a.y[c] = placeRes.y[c];

                }
                var t = new ChipTask(d, a) { Height = 50/*sizes[i].Height*/, Width = 50/*sizes[i].Width*/ };

                t.Save(Path.Combine(resultDirectory, string.Format("TestData {0}.bin", testCount)));

                st.Stop();
                Console.WriteLine(@"Time for {0} - {1}", placer, st.Elapsed);
            }
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
    }
}