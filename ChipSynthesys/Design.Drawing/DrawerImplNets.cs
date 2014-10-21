using System.Drawing.Drawing2D;
using PlaceModel;
using System.Drawing;

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
            float scaling = size.Width / design.field.cellsx < size.Height / design.field.cellsy
                              ? (float)size.Width / design.field.cellsx
                              : (float)size.Height / design.field.cellsy;
            //scaling = scaling < 2 ? 2 : scaling;

            Brush br = new SolidBrush(Color.LightGray);
            Pen pen = new Pen(Color.Red, PenThickness);
            Pen framePen = new Pen(Color.Black, 1);
            Brush brBlack = new SolidBrush(Color.Black);

            // рисуем границу
            canvas.DrawRectangle(framePen, 0, 0, design.field.cellsx * scaling - 1, design.field.cellsy * scaling - 1);

            double[] drawnNetsY = new double[design.nets.Length];

            // рисуем элементы
            foreach (var c in design.components)
            {
                if (placement.placed[c])
                {
                    canvas.DrawRectangle(pen, (float)(placement.x[c] * scaling),
                                         (float)(placement.y[c] * scaling), c.sizex * scaling, c.sizey * scaling);

                    canvas.FillRectangle(br,
                                         (float)(placement.x[c] * scaling) + PenThickness - 1,
                                         (float)(placement.y[c] * scaling) + PenThickness - 1,
                                         c.sizex * scaling - PenThickness,
                                         c.sizey * scaling - PenThickness);

                    // проверить как отображается
                    canvas.FillEllipse(brBlack, (float)(placement.x[c] * scaling - dotSize / 2),
                                       (float)(placement.y[c] * scaling - dotSize / 2), dotSize, dotSize);
                }
            }

            // вычислить и нарисовать цепь
            for (int i = 0; i < design.nets.Length; i++)
            {
                if (design.nets[i].items.Length == 0)
                {
                    continue;
                }

                // для определения границ по х центральной линии масс
                float xMin = design.field.cellsx * scaling;
                float xMax = 0;

                // сколько разместили
                int placeInNet = 0;
                // проходим по всем компонентам сети
                foreach (var comp in design.nets[i].items)
                {
                    if (placement.placed[comp])
                    {
                        if (placement.x[comp] * scaling + (float)comp.sizex * scaling / 2 < xMin)
                            xMin = (float)placement.x[comp] * scaling + (float)comp.sizex * scaling / 2;
                        if (placement.x[comp] * scaling + (float)comp.sizex * scaling / 2 > xMax)
                            xMax = (float)placement.x[comp] * scaling + (float)comp.sizex * scaling / 2;

                        drawnNetsY[i] += placement.y[comp];
                        placeInNet++;
                    }
                }
                if (placeInNet == 0)
                {
                    continue;

                }

                // сосчитали среднее для одной цепи => ищем среднее
                drawnNetsY[i] = drawnNetsY[i] / placeInNet * scaling;

                // рисуем линию горизонтальную, проходящую через центр масс
                canvas.DrawLine(framePen, xMin, (int)drawnNetsY[i], xMax, (int)drawnNetsY[i]);

                //соединяем компоненты с линией масс
                foreach (var comp in design.nets[i].items)
                {
                    // проверяем, размещена ли компонент
                    if (placement.placed[comp])
                    {
                        canvas.DrawLine(framePen,
                                        (float)placement.x[comp] * scaling + (float)comp.sizex / 2 * scaling,
                                        (float)placement.y[comp] * scaling + (float)comp.sizey / 2 * scaling,
                                        (float)placement.x[comp] * scaling + (float)comp.sizex / 2 * scaling,
                                        (float)drawnNetsY[i]);

                        // чтобы уменьшить размер точек - делим радиус (два параметра в конце)
                        // и делим на такое же число отступ для правильного позиционирования центра
                        canvas.FillEllipse(brBlack,
                                           (float)
                                           (placement.x[comp] * scaling + (float)comp.sizex * scaling / 2 - dotSize / 2),
                                           (float)
                                           (placement.y[comp] * scaling + (float)comp.sizey * scaling / 2 - dotSize / 2),
                                           dotSize, dotSize);
                    }
                }
            }
        }

        public override void Draw(PlaceModel.Design design, PlacementDetail placement, Size size,
                                  System.Drawing.Graphics canvas)
        {
            float scaling = size.Width / design.field.cellsx < size.Height / design.field.cellsy
                              ? (float)size.Width / design.field.cellsx
                              : (float)size.Height / design.field.cellsy;

            Brush br = new SolidBrush(Color.LightGray);
            Pen pen = new Pen(Color.Red, PenThickness);
            Pen penNp = new Pen(Color.Orange, PenThickness);
            Pen framePen = new Pen(Color.Black, 1);
            Brush brBlack = new SolidBrush(Color.Black);

            // для центра масс по координатам
            int[] drawnNetsY = new int[design.nets.Length];

            // рисуем границу
            canvas.DrawRectangle(framePen, 0f, 0f, design.field.cellsx * scaling - 1, design.field.cellsy * scaling - 1);

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
                else
                {
                    canvas.DrawRectangle(penNp, placement.x[c] * scaling,
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
                if (design.nets[i].items.Length == 0)
                {
                    continue;
                }
                // для определения границ по х центральной линии масс
                float xMin = design.field.cellsx * scaling;
                float xMax = 0;

                // сколько разместили
                int placeInNet = 0;
                // проходим по всем компонентам сети
                foreach (var comp in design.nets[i].items)
                {
                    if (placement.placed[comp])
                    {
                        if (placement.x[comp]*scaling + comp.sizex*scaling/2 < xMin)
                        {
                            xMin = placement.x[comp] * scaling + comp.sizex * scaling / 2;
                        }
                        if (placement.x[comp]*scaling + comp.sizex*scaling/2 > xMax)
                        {
                            xMax = placement.x[comp] * scaling + comp.sizex * scaling / 2;
                        }
                    }

                    drawnNetsY[i] += placement.y[comp];
                    placeInNet++;
                }

                // сосчитали среднее для одной цепи => ищем среднее
                drawnNetsY[i] = (int)(drawnNetsY[i]  * scaling / placeInNet);

                // рисуем линию горизонтальную, проходящую через центр масс
                canvas.DrawLine(framePen, xMin, drawnNetsY[i], xMax, drawnNetsY[i]);

                //соединяем компоненты с линией масс
                foreach (var comp in design.nets[i].items)
                {
                    if (placement.placed[comp])
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
}