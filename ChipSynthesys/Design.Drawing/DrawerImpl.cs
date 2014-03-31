using System.Drawing;
using PlaceModel;

namespace ChipSynthesys.Draw
{
    public class DrawerImpl : DrawerBase
    {
        private short PenThickness = 3;
        private const short dotSize = 8;                     // можно поменять


        public override void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas)
        {
            int scaling = size.Width / design.field.cellsx < size.Height / design.field.cellsy ? size.Width / design.field.cellsx : size.Height / design.field.cellsy;
            Brush br = new SolidBrush(Color.LightGray);
            Pen pen = new Pen(Color.Red, PenThickness);
            Brush brBlack = new SolidBrush(Color.Black);

            // рисуем границу
            canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, design.field.cellsx * scaling - 1, design.field.cellsy * scaling - 1);

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


                    canvas.FillEllipse(brBlack, (int)(placement.x[c] * scaling - dotSize / 2), (int)(placement.y[c] * scaling - dotSize / 2), dotSize, dotSize);
                }
            }

        }

        public override void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas)
        {
            int scaling = size.Width / design.field.cellsx < size.Height / design.field.cellsy ? size.Width / design.field.cellsx : size.Height / design.field.cellsy;
            Brush br = new SolidBrush(Color.LightGray);
            Pen pen = new Pen(Color.Red, PenThickness);
            Brush brBlack = new SolidBrush(Color.Black);

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


                    canvas.FillEllipse(brBlack, placement.x[c] * scaling - dotSize / 2, placement.y[c] * scaling - dotSize / 2, dotSize, dotSize);
                }
            }

        }
    }
}
