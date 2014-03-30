using System.Drawing;
using PlaceModel;

namespace ChipSynthesys.Draw
{
    /// <summary>
    /// Рисуем компоненты вместе с цепями
    /// </summary>
    public class DrawerImplNets : DrawerBase
    {
        private short PenThickness = 3;
        private const short dotSize = 8;


        public override void Draw(PlaceModel.Design design, PlacementGlobal placement, Size size,
            System.Drawing.Graphics canvas)
        {
            int scaling = size.Width / design.field.cellsx > size.Height / design.field.cellsy
                              ? size.Width / design.field.cellsx
                              : size.Height / design.field.cellsy;

            Brush br = new SolidBrush(Color.LightGray);
            Pen pen = new Pen(Color.Red, PenThickness);
            Pen framePen = new Pen(Color.Black, 1);
            Brush brBlack = new SolidBrush(Color.Black);

            // рисуем границу
            canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, design.field.cellsx * scaling - 1, design.field.cellsy * scaling - 1);


            double[] drawnNetsY = new double[design.nets.Length];

            // рисуем элементы
            foreach (var c in design.components)
            {
                if (placement.placed[c])
                {
                    canvas.DrawRectangle(pen, (int)(placement.x[c] * scaling),
                                         (int)(placement.y[c] * scaling), c.sizex * scaling, c.sizey * scaling);


                    canvas.FillRectangle(br,
                                         (int)(placement.x[c] * scaling) + PenThickness - 1,
                                         (int)(placement.y[c] * scaling) + PenThickness - 1,
                                         c.sizex * scaling - PenThickness,
                                         c.sizey * scaling - PenThickness);

                    // проверить как отображается
                    canvas.FillEllipse(brBlack, (int)(placement.x[c] * scaling - dotSize / 2),
                                       (int)(placement.y[c] * scaling - dotSize / 2), dotSize, dotSize);
                }
            }


            // вычислить и нарисовать цепь
            for (int i = 0; i < design.nets.Length; i++)
            {
                // для определения границ по х центральной линии масс 
                int xMin = design.field.cellsx * scaling;
                int xMax = 0;


                // проходим по всем компонентам сети 
                foreach (var comp in design.nets[i].items)
                {
                    if (placement.x[comp] * scaling + comp.sizex * scaling / 2 < xMin)
                        xMin = (int)placement.x[comp] * scaling + comp.sizex * scaling / 2;
                    if (placement.x[comp] * scaling + comp.sizex * scaling / 2 > xMax)
                        xMax = (int)placement.x[comp] * scaling + comp.sizex * scaling / 2;

                    drawnNetsY[i] += placement.y[comp];
                }

                // сосчитали среднее для одной цепи => ищем среднее
                drawnNetsY[i] = drawnNetsY[i] / design.nets.Length * scaling;

                // рисуем линию горизонтальную, проходящую через центр масс
                canvas.DrawLine(framePen, xMin, (int)drawnNetsY[i], xMax, (int)drawnNetsY[i]);

                //соединяем компоненты с линией масс
                foreach (var comp in design.nets[i].items)
                {
                    canvas.DrawLine(framePen,
                        (int)placement.x[comp] * scaling + comp.sizex / 2 * scaling,
                        (int)placement.y[comp] * scaling + comp.sizey / 2 * scaling,
                        (int)placement.x[comp] * scaling + comp.sizex / 2 * scaling,
                        (int)drawnNetsY[i]);

                    // чтобы уменьшить размер точек - делим радиус (два параметра в конце)
                    // и делим на такое же число отступ для правильного позиционирования центра
                    canvas.FillEllipse(brBlack,
                        (int)(placement.x[comp] * scaling + comp.sizex * scaling / 2 - dotSize / 2),
                        (int)(placement.y[comp] * scaling + comp.sizey * scaling / 2 - dotSize / 2),
                    dotSize, dotSize);
                }
            }


        }


        public override void Draw(PlaceModel.Design design, PlacementDetail placement, Size size,
                                  System.Drawing.Graphics canvas)
        {
            int scaling = size.Width / design.field.cellsx > size.Height / design.field.cellsy
                              ? size.Width / design.field.cellsx
                              : size.Height / design.field.cellsy;

            Brush br = new SolidBrush(Color.LightGray);
            Pen pen = new Pen(Color.Red, PenThickness);
            Pen framePen = new Pen(Color.Black, 1);
            Brush brBlack = new SolidBrush(Color.Black);

            // для центра масс по координатам
            int[] drawnNetsY = new int[design.nets.Length];


            // рисуем границу
            canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, design.field.cellsx * scaling - 1, design.field.cellsy * scaling - 1);



            // рисуем элементы
            foreach (var c in design.components)
            {
                if (placement.placed[c])
                {
                    canvas.DrawRectangle(pen, placement.x[c] * scaling,
                                         placement.y[c] * scaling, c.sizex * scaling, c.sizey * scaling);


                    canvas.FillRectangle(br,
                                         placement.x[c] * scaling + PenThickness - 1,
                                         placement.y[c] * scaling + PenThickness - 1,
                                         c.sizex * scaling - PenThickness,
                                         c.sizey * scaling - PenThickness);

                    // проверить как отображается
                    canvas.FillEllipse(brBlack, placement.x[c] * scaling - dotSize / 2, placement.y[c] * scaling - dotSize / 2,
                                       dotSize, dotSize);
                }
            }

            // вычислить и нарисовать цепь
            for (int i = 0; i < design.nets.Length; i++)
            {
                // для определения границ по х центральной линии масс 
                int xMin = design.field.cellsx * scaling;
                int xMax = 0;


                // проходим по всем компонентам сети 
                foreach (var comp in design.nets[i].items)
                {
                    if (placement.x[comp] * scaling + comp.sizex * scaling / 2 < xMin)
                        xMin = placement.x[comp] * scaling + comp.sizex * scaling / 2;
                    if (placement.x[comp] * scaling + comp.sizex * scaling / 2 > xMax)
                        xMax = placement.x[comp] * scaling + comp.sizex * scaling / 2;

                    drawnNetsY[i] += placement.y[comp];
                }

                // сосчитали среднее для одной цепи => ищем среднее
                drawnNetsY[i] = drawnNetsY[i] / design.nets[i].items.Length * scaling;


                // рисуем линию горизонтальную, проходящую через центр масс
                canvas.DrawLine(framePen, xMin, drawnNetsY[i], xMax, drawnNetsY[i]);

                //соединяем компоненты с линией масс
                foreach (var comp in design.nets[i].items)
                {
                    canvas.DrawLine(framePen,
                                    placement.x[comp] * scaling + comp.sizex * scaling / 2,
                                    placement.y[comp] * scaling + comp.sizey * scaling / 2,
                                    placement.x[comp] * scaling + comp.sizex * scaling / 2,
                                    drawnNetsY[i]);

                    // чтобы уменьшить размер точек - делим радиус (два параметра в конце)
                    // и делим на такое же число отступ для правильного позиционирования центра
                    canvas.FillEllipse(brBlack,
                        placement.x[comp] * scaling + comp.sizex * scaling / 2 - dotSize / 2,
                        placement.y[comp] * scaling + comp.sizey * scaling / 2 - dotSize / 2,
                    dotSize, dotSize);
                }
            }
        }
    }
}
