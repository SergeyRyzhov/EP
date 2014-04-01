using System;
using System.Drawing;
using System.IO;
using ChipSynthesys.Common.Generators;
using PlaceModel;

namespace TestRunner
{
    public class Program
    {
        static void Main(string[] args)
        {
            Design[] design;
            PlacementGlobal[] approximate;
            Size[] sizes;
            Bitmap[] bitmaps;
            ReadInput(args, out design, out approximate, out sizes, out bitmaps);

            //запуск тестов
            //запись статистики
            //создать папку в ней всй сохранить
            //наверное лучше каждую статику отдельно
            //но ещё и некоторое полное описание всех тестов

        }

        private static void ReadInput(string[] args, out Design[] design, out PlacementGlobal[] approximate, out Size[] sizes, out Bitmap[] bitmaps)
        {
            if (args.Length == 1)
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
