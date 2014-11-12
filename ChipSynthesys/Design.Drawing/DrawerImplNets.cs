using System.Drawing;

using PlaceModel;

namespace ChipSynthesys.Draw
{
    /// <summary>
    ///     Рисуем компоненты вместе с цепями
    /// </summary>
    public class DrawerImplNets : DrawerBase
    {
        private const short dotSize = 8;

        private short PenThickness = 3;

        public override void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas)
        {
            float scaling = size.Width / design.field.cellsx < size.Height / design.field.cellsy
                                ? size.Width / design.field.cellsx
                                : size.Height / design.field.cellsy;

            var framePen = new Pen(Color.Black, 1);
            Brush br = new SolidBrush(Color.DarkGray);
            var pen = new Pen(Color.Green, this.PenThickness);
            Brush brBlack = new SolidBrush(Color.Black);

            Brush notPlacedBr = new SolidBrush(Color.LightGray);
            var notPlacedPen = new Pen(Color.Red, this.PenThickness);
            Brush notPlacedBrBlack = new SolidBrush(Color.Black);

            // рисуем границу
            canvas.DrawRectangle(framePen, 0, 0, design.field.cellsx * scaling - 1, design.field.cellsy * scaling - 1);

            var drawnNetsY = new double[design.nets.Length];

            // рисуем элементы
            foreach (Component c in design.components)
            {
                if (placement.placed[c])
                {
                    canvas.DrawRectangle(
                        pen,
                        (float)(placement.x[c] * scaling),
                        (float)(placement.y[c] * scaling),
                        c.sizex * scaling,
                        c.sizey * scaling);

                    canvas.FillRectangle(
                        br,
                        (float)(placement.x[c] * scaling) + this.PenThickness - 1,
                        (float)(placement.y[c] * scaling) + this.PenThickness - 1,
                        c.sizex * scaling - this.PenThickness,
                        c.sizey * scaling - this.PenThickness);

                    // проверить как отображается
                    canvas.FillEllipse(
                        brBlack,
                        (float)(placement.x[c] * scaling - dotSize / 2),
                        (float)(placement.y[c] * scaling - dotSize / 2),
                        dotSize,
                        dotSize);
                }
                else
                {
                    canvas.DrawRectangle(
                        notPlacedPen,
                        (float)(placement.x[c] * scaling),
                        (float)(placement.y[c] * scaling),
                        c.sizex * scaling,
                        c.sizey * scaling);

                    canvas.FillRectangle(
                        notPlacedBr,
                        (float)(placement.x[c] * scaling) + this.PenThickness - 1,
                        (float)(placement.y[c] * scaling) + this.PenThickness - 1,
                        c.sizex * scaling - this.PenThickness,
                        c.sizey * scaling - this.PenThickness);

                    // проверить как отображается
                    canvas.FillEllipse(
                        notPlacedBrBlack,
                        (float)(placement.x[c] * scaling - dotSize / 2),
                        (float)(placement.y[c] * scaling - dotSize / 2),
                        dotSize,
                        dotSize);
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
                foreach (Component comp in design.nets[i].items)
                {
                    if (placement.placed[comp])
                    {
                        if (placement.x[comp] * scaling + comp.sizex * scaling / 2 < xMin)
                        {
                            xMin = (float)placement.x[comp] * scaling + comp.sizex * scaling / 2;
                        }
                        if (placement.x[comp] * scaling + comp.sizex * scaling / 2 > xMax)
                        {
                            xMax = (float)placement.x[comp] * scaling + comp.sizex * scaling / 2;
                        }

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
                foreach (Component comp in design.nets[i].items)
                {
                    // проверяем, размещена ли компонент
                    if (placement.placed[comp])
                    {
                        canvas.DrawLine(
                            framePen,
                            (float)placement.x[comp] * scaling + (float)comp.sizex / 2 * scaling,
                            (float)placement.y[comp] * scaling + (float)comp.sizey / 2 * scaling,
                            (float)placement.x[comp] * scaling + (float)comp.sizex / 2 * scaling,
                            (float)drawnNetsY[i]);

                        // чтобы уменьшить размер точек - делим радиус (два параметра в конце)
                        // и делим на такое же число отступ для правильного позиционирования центра
                        canvas.FillEllipse(
                            brBlack,
                            (float)(placement.x[comp] * scaling + comp.sizex * scaling / 2 - dotSize / 2),
                            (float)(placement.y[comp] * scaling + comp.sizey * scaling / 2 - dotSize / 2),
                            dotSize,
                            dotSize);
                    }
                }
            }
        }

        public override void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas)
        {
            float scaling = size.Width / design.field.cellsx < size.Height / design.field.cellsy
                                ? size.Width / design.field.cellsx
                                : size.Height / design.field.cellsy;

            Brush br = new SolidBrush(Color.LightGray);
            var pen = new Pen(Color.Red, this.PenThickness);
            var penNp = new Pen(Color.Orange, this.PenThickness);
            var framePen = new Pen(Color.Black, 1);
            Brush brBlack = new SolidBrush(Color.Black);

            // для центра масс по координатам
            var drawnNetsY = new float[design.nets.Length];

            // рисуем границу
            canvas.DrawRectangle(framePen, 0, 0, design.field.cellsx * scaling - 1, design.field.cellsy * scaling - 1);

            // рисуем элементы
            foreach (Component c in design.components)
            {
                if (placement.placed[c])
                {
                    canvas.DrawRectangle(
                        pen,
                        placement.x[c] * scaling,
                        placement.y[c] * scaling,
                        c.sizex * scaling,
                        c.sizey * scaling);

                    canvas.FillRectangle(
                        br,
                        placement.x[c] * scaling + this.PenThickness - 1,
                        placement.y[c] * scaling + this.PenThickness - 1,
                        c.sizex * scaling - this.PenThickness,
                        c.sizey * scaling - this.PenThickness);

                    // проверить как отображается
                    canvas.FillEllipse(
                        brBlack,
                        placement.x[c] * scaling - dotSize / 2,
                        placement.y[c] * scaling - dotSize / 2,
                        dotSize,
                        dotSize);
                }
                else
                {
                    canvas.DrawRectangle(
                        penNp,
                        placement.x[c] * scaling,
                        placement.y[c] * scaling,
                        c.sizex * scaling,
                        c.sizey * scaling);

                    canvas.FillRectangle(
                        br,
                        placement.x[c] * scaling + this.PenThickness - 1,
                        placement.y[c] * scaling + this.PenThickness - 1,
                        c.sizex * scaling - this.PenThickness,
                        c.sizey * scaling - this.PenThickness);

                    // проверить как отображается
                    canvas.FillEllipse(
                        brBlack,
                        placement.x[c] * scaling - dotSize / 2,
                        placement.y[c] * scaling - dotSize / 2,
                        dotSize,
                        dotSize);
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
                foreach (Component comp in design.nets[i].items)
                {
                    if (placement.placed[comp])
                    {
                        if (placement.x[comp] * scaling + comp.sizex * scaling / 2 < xMin)
                        {
                            xMin = placement.x[comp] * scaling + comp.sizex * scaling / 2;
                        }
                        if (placement.x[comp] * scaling + comp.sizex * scaling / 2 > xMax)
                        {
                            xMax = placement.x[comp] * scaling + comp.sizex * scaling / 2;
                        }
                    }

                    drawnNetsY[i] += placement.y[comp];
                    placeInNet++;
                }

                // сосчитали среднее для одной цепи => ищем среднее
                drawnNetsY[i] = drawnNetsY[i] / placeInNet * scaling;

                // рисуем линию горизонтальную, проходящую через центр масс
                canvas.DrawLine(framePen, xMin, drawnNetsY[i], xMax, drawnNetsY[i]);

                //соединяем компоненты с линией масс
                foreach (Component comp in design.nets[i].items)
                {
                    if (placement.placed[comp])
                    {
                        canvas.DrawLine(
                            framePen,
                            placement.x[comp] * scaling + comp.sizex * scaling / 2,
                            placement.y[comp] * scaling + comp.sizey * scaling / 2,
                            placement.x[comp] * scaling + comp.sizex * scaling / 2,
                            drawnNetsY[i]);

                        // чтобы уменьшить размер точек - делим радиус (два параметра в конце)
                        // и делим на такое же число отступ для правильного позиционирования центра
                        canvas.FillEllipse(
                            brBlack,
                            placement.x[comp] * scaling + comp.sizex * scaling / 2 - dotSize / 2,
                            placement.y[comp] * scaling + comp.sizey * scaling / 2 - dotSize / 2,
                            dotSize,
                            dotSize);
                    }
                }
            }
        }

        public override void DrawRect(
            Design design,
            PlacementDetail placement,
            Size size,
            Graphics canvas,
            int x,
            int y,
            int width,
            int height)
        {
            float scaling = this.getScaling(size.Width, size.Height, width, height);
            float penBorderDepth = 1;

            Brush br = new SolidBrush(Color.DarkGray);
            var pen = new Pen(Color.Green, this.PenThickness);

            Brush notPlacedBr = new SolidBrush(Color.LightGray);
            var notPlacedPen = new Pen(Color.Red, this.PenThickness);

            // rect for determing intersect with elements that are not fully embraced by the area of drawing
            var scaleRect = new Rectangle(x, y, width, height);

            canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, width * scaling - 1, height * scaling - 1);

            foreach (Component c in design.components)
            {
                if (placement.placed[c])
                {
                    var currentComponent = new Rectangle(placement.x[c], placement.y[c], c.sizex, c.sizey);

                    if (scaleRect.IntersectsWith(currentComponent))
                    {
                        canvas.DrawRectangle(
                            pen,
                            (placement.x[c] - x) * scaling,
                            (placement.y[c] - y) * scaling,
                            c.sizex * scaling,
                            c.sizey * scaling);

                        canvas.FillRectangle(
                            br,
                            (placement.x[c] - x) * scaling + penBorderDepth,
                            (placement.y[c] - y) * scaling + penBorderDepth,
                            c.sizex * scaling - penBorderDepth,
                            c.sizey * scaling - penBorderDepth);
                    }
                }
                else
                {
                    canvas.DrawRectangle(
                        notPlacedPen,
                        (placement.x[c] - x) * scaling,
                        (placement.y[c] - y) * scaling,
                        c.sizex * scaling,
                        c.sizey * scaling);

                    canvas.FillRectangle(
                        notPlacedBr,
                        (placement.x[c] - x) * scaling + penBorderDepth,
                        (placement.y[c] - y) * scaling + penBorderDepth,
                        c.sizex * scaling - penBorderDepth,
                        c.sizey * scaling - penBorderDepth);
                }
            }
        }

        public override void DrawRect(
            Design design,
            PlacementGlobal placement,
            Size size,
            Graphics canvas,
            int x,
            int y,
            int width,
            int height)
        {
            float scaling = this.getScaling(size.Width, size.Height, width, height);
            float penBorderDepth = 1;

            Brush br = new SolidBrush(Color.DarkGray);
            var pen = new Pen(Color.Green, 1);

            Brush notPlacedBr = new SolidBrush(Color.LightGray);
            var notPlacedPen = new Pen(Color.Red, this.PenThickness);

            // rect for determing intersect with elements that are not fully embraced by the area of drawing
            var scaleRect = new Rectangle(x, y, width, height);

            canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, width * scaling - 1, height * scaling - 1);

            foreach (Component c in design.components)
            {
                if (placement.placed[c])
                {
                    var currentComponent = new Rectangle((int)placement.x[c], (int)placement.y[c], c.sizex, c.sizey);

                    if (scaleRect.IntersectsWith(currentComponent))
                    {
                        canvas.DrawRectangle(
                            pen,
                            (float)(placement.x[c] - x) * scaling,
                            (float)(placement.y[c] - y) * scaling,
                            c.sizex * scaling,
                            c.sizey * scaling);

                        canvas.FillRectangle(
                            br,
                            (float)(placement.x[c] - x) * scaling + penBorderDepth,
                            (float)(placement.y[c] - y) * scaling + penBorderDepth,
                            c.sizex * scaling - penBorderDepth,
                            c.sizey * scaling - penBorderDepth);
                    }
                }
                else
                {
                    canvas.DrawRectangle(
                        notPlacedPen,
                        (float)(placement.x[c] - x) * scaling,
                        (float)(placement.y[c] - y) * scaling,
                        c.sizex * scaling,
                        c.sizey * scaling);

                    canvas.FillRectangle(
                        notPlacedBr,
                        (float)(placement.x[c] - x) * scaling + penBorderDepth,
                        (float)(placement.y[c] - y) * scaling + penBorderDepth,
                        c.sizex * scaling - penBorderDepth,
                        c.sizey * scaling - penBorderDepth);
                }
            }
        }

        private float getScaling(float swidth, float sheight, float width, float height)
        {
            float scaling = swidth / width < sheight / height ? swidth / width : sheight / height;
            return scaling;
        }
    }
}