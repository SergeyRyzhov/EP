using System;
using System.Collections.Generic;
using System.Drawing;
using PlaceModel;

namespace ChipSynthesys.Draw
{
    public class DrawerImpl : DrawerBase
    {
        // bitmap создан здесь для тестов
        internal Bitmap b1;
        private const short scale = 20;
        private const short otstup = 20;
        private static ushort NumberOfPictres = 0;
        private short PenThickness = 3;

        public override void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas)
        {
            Random r = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            int countNets = design.nets.Length;                                              // для каждой сети свой цвет
            Dictionary<int, Pen> penMap = new Dictionary<int, Pen>();
            Brush br = new SolidBrush(Color.Gold);

            // перенести сюда объявление bitmap
            b1 = new Bitmap(design.field.cellsx * 50 + 1, design.field.cellsy * 50 + 1);



            foreach (var net in design.nets)
            {
                penMap.Add(net.id, new Pen(Color.FromArgb(r.Next(255), r.Next(255), r.Next(255), r.Next(255)), PenThickness)); // толщина была 8
            }


            using (canvas = Graphics.FromImage(b1))
            {
                // рисуем границу
                canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, design.field.cellsx * 50, design.field.cellsy * 50);

                foreach (var c in design.components)
                {
                    if (placement.placed[c])
                    {
                        // проходим в цикле по полученному списку сетей 
                        // и получаем по id каждой сети перо их penMap
                        for (int i = 0; i < design.Nets(c).Length; i++)
                        {
                            canvas.DrawRectangle(penMap[design.Nets(c)[i].id], (int)placement.x[c] * scale + i * PenThickness,
                                                 (int)placement.y[c] * scale + i * PenThickness, c.sizex * scale - 2 * i * PenThickness, c.sizey * scale - 2 * i * PenThickness);
                        }

                        canvas.FillRectangle(br,
                            (int)placement.x[c] * scale + design.Nets(c).Length * PenThickness,
                            (int)placement.y[c] * scale + design.Nets(c).Length * PenThickness,
                            c.sizex * scale - 2 * design.Nets(c).Length * PenThickness,
                            c.sizey * scale - 2 * design.Nets(c).Length * PenThickness);
                    }
                }
            }

            // сохраняем файл
            var file = string.Format("{0}", NumberOfPictres++);
            b1.Save("test" + file + ".png");
        }

        public override void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas)
        {
            Random r = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            int countNets = design.nets.Length;                                              // для каждой сети свой цвет
            Dictionary<int, Pen> penMap = new Dictionary<int, Pen>();
            Brush br = new SolidBrush(Color.Gold);

            // перенести сюда объявление bitmap
            //b1 = new Bitmap(design.field.cellsx * 50 + 1, design.field.cellsy * 50 + 1);



            foreach (var net in design.nets)
            {
                //penMap.Add(net.id, new Pen(Color.FromArgb(r.Next(255), r.Next(255), r.Next(255), r.Next(255)), PenThickness)); // толщина была 8
                penMap.Add(net.id, new Pen(Color.Red, 1)); // толщина была 8
            }


            //using (canvas = Graphics.FromImage(b1))
            //{
                // рисуем границу
                canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, design.field.cellsx * 50, design.field.cellsy * 50);

                foreach (var c in design.components)
                {
                    if (placement.placed[c])
                    {
                        // проходим в цикле по полученному списку сетей 
                        // и получаем по id каждой сети перо их penMap
                        
                        for (int i = 0; i < design.Nets(c).Length; i++)
                        {
                            canvas.DrawRectangle(penMap[design.Nets(c)[i].id], (int)placement.x[c] * scale + i * PenThickness,
                                                 (int)placement.y[c] * scale + i * PenThickness, c.sizex * scale - 2 * i * PenThickness, c.sizey * scale - 2 * i * PenThickness);
                        }

                        canvas.FillRectangle(br,
                            (int)placement.x[c] * scale + design.Nets(c).Length * PenThickness+1,
                            (int)placement.y[c] * scale + design.Nets(c).Length * PenThickness+1,
                            c.sizex * scale - 2 * design.Nets(c).Length * PenThickness-1,
                            c.sizey * scale - 2 * design.Nets(c).Length * PenThickness-1);
                    }
                }
            //}

            // сохраняем файл
            //var file = string.Format("{0}", NumberOfPictres++);
            //b1.Save("test" + file + ".bmp");
        }
    }
}
