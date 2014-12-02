using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ChipSynthesys.Common;
using ChipSynthesys.Common.Classes;
using ChipSynthesys.Common.Generators;
using ChipSynthesys.Draw;
using ChipSynthesys.Statistic;
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
        private static ChipTask[] GenerateTestData()
        {
            var tasks = new ChipTask[TestsConstants.TestTasksCount];
            for (int i = 0; i < TestsConstants.TestTasksCount; i++)
            {
                tasks[i] = GenerateTestDesign(string.Format(@"Generated {0}", i + 1));
            }
            return tasks;
        }

        private static ChipTask GenerateTestDesign(string name)
        {
            IGenerator generator = new DenseGenerator();
            const int n = 250; //число компонент
            const int maxx = 7; //размер по x
            const int maxy = 6; //размер по y
            const int p = 70; //процент заполнения
            const int nets = 1000; //число сетей
            const int maxNetSize = 9; //длина цепей
            Design design;
            PlacementGlobal placement;
            generator.NextDesignWithPlacement(n, nets, maxNetSize, p, maxx, maxy, 0, 0, out design, out placement);
            return new ChipTask(name, design, placement);
        }

        private static ChipTask[] LoadFromDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            var tasks = new List<ChipTask>();
            foreach (string fileName in files)
            {
                string extension = Path.GetExtension(fileName);
                if (extension != null && extension.Equals(@".bin", StringComparison.InvariantCultureIgnoreCase))
                {
                    tasks.Add(ChipTask.Load(fileName));
                }
            }

            return tasks.ToArray();
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

            string tasksDirectory = args.Length > 0 ? args[0] : @".\";
            string path = tasksDirectory + @"Tests\";
            Directory.CreateDirectory(path);

            ChipTask[] tasks = ReadInput(tasksDirectory);

            var statistic = new CommonStatistic();
            int testCount = 0;
            var masType = new Type[] { };

            //пока не используются
            //RunCommonTests(designs, statistic, resultDirectory, globalPlacements, sizes, bitmaps, ref testCount);

            IDetailPlacer[] otherPlacers =
                {
//                    new ForceDirectedDetailPlacer(5),
//                    new ForceDirectedDetailPlacer(20), new ForceDirectedDetailPlacer(25),
                    new CombinePlacer(), 
                    new CrossReductPlacer(), 
                    new CrossComponentVariant2(),
//                    new CrossCompPlacer(), 
                };

            for (int i = 0; i < otherPlacers.Length; i++)
            {
                IDetailPlacer placer = otherPlacers[i];
                var methodPath = Path.Combine(path, i + placer.GetType().ToString());
                if (!Directory.Exists(methodPath))
                {
                    Directory.CreateDirectory(methodPath);
                }

                Parallel.ForEach(tasks, task => PlacementTask(task, methodPath, statistic, placer));
                testCount += tasks.Length;
            }
        }

        private static void PlacementTask(
            ChipTask task,
            string path,
            CommonStatistic statistic,
            IDetailPlacer placer)
        {
            try
            {
                Design d = task.Design;
                PlacementDetail placeRes;

                task.SimpleDraw(Path.Combine(path, string.Format(@"{0} Before", task.Name)));

                PlacementGlobal taskPlacement;
                if (task.CurrentPlacement == null)
                {
                    taskPlacement = task.GlobalPlacement;
                }
                else
                {
                    taskPlacement = new PlacementGlobal(task.Design);
                    foreach (var component in task.Design.components)
                    {
                        taskPlacement.placed[component] = task.GlobalPlacement.placed[component];
                        taskPlacement.x[component] = task.CurrentPlacement.x[component];
                        taskPlacement.y[component] = task.CurrentPlacement.y[component];
                    }
                }

                var statisticResult = statistic.Compute(task);
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                placer.Place(d, taskPlacement, out placeRes);
                stopwatch.Stop();
                statisticResult = statistic.Update(statisticResult, task, placeRes, stopwatch.Elapsed);

                string fileName = Path.Combine(path, string.Format(@"{0} TestResult.xlsx", task.Name));
                StatisticImporter.SaveToFile(
                    fileName,
                    statisticResult,
                    task.Name,
                    placer.ToString());

                var chipTask = task.Clone();
                chipTask.CurrentPlacement = placeRes;
                chipTask.SimpleDraw(Path.Combine(path, string.Format(@"{0} After", task.Name)));

                chipTask.Save(Path.Combine(path, string.Format(@"{0} Placed.bin", task.Name)));

                Console.WriteLine(@"Time for {0} - {1}", placer, stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
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
                drawer.Draw(task.Design, task.GlobalPlacement, new Size(w, h), canvas);
            }

            string file = string.Format(@"{0}.png", taskName);
            bitmap.Save(file);
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
                        drawer.DrawRect(task.Design, task.GlobalPlacement, new Size(size, size), canvas, x, y, dw, dh);
                    }

                    string file = string.Format(@"{0}-{1}-{2}.png", taskName, x, y);
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

        private static ChipTask[] ReadInput(string taskDirectory)
        {
            ChipTask[] tasks = LoadFromDirectory(taskDirectory);

            if (tasks.Length == 0)
            {
                tasks = GenerateTestData();
            }

            return tasks;
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
                                                posSorter);
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
            IPositionsSorter posSorter)
        {
            var placer = new DetailPlacerImpl(compOrder, posSearcher, posSorter);

            for (int i = 0; i < design.Length; i++)
            {
                var st = new Stopwatch();
                st.Start();
                Design d = design[i];
                PlacementDetail placeRes;
                testCount++;
                string imageBefore = Path.Combine(resultDirectory, string.Format(@"TestData {0}.png", testCount));
                //DrawerHelper.SimpleDraw(design[i], approximate[i], sizes[i], bitmaps[i], imageBefore);

                placer.Place(d, approximate[i], out placeRes);

                //var statisticResult = statistic.Compute(d, approximate[i], placeRes);
                //statisticResult = statistic.Update(statisticResult, d, approximate[i], placeRes);

                string fileName = Path.Combine(resultDirectory, string.Format(@"TestResult {0}.xlsx", testCount));
                /*StatisticImporter.SaveToFile(
                    fileName,
                    statisticResult,
                    i.ToString(CultureInfo.InvariantCulture),
                    placer.ToString());*/

                string imageAfter = Path.Combine(resultDirectory, string.Format(@"TestResult {0}.png", testCount));
                // DrawerHelper.SimpleDraw(design[i], placeRes, sizes[i], bitmaps[i], imageAfter);

                //var t = new ChipTask(d, approximate[i], placeRes);

                // t.Save(Path.Combine(resultDirectory, string.Format(@"TestData {0}.bin", testCount)));

                st.Stop();
                Console.WriteLine(@"Time for {0} - {1}", placer, st.Elapsed);
            }
        }
    }
}