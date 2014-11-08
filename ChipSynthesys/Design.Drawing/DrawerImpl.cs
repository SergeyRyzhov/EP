using PlaceModel;
using System.Drawing;
using System;

namespace ChipSynthesys.Draw
{
    public class DrawerImpl : DrawerBase
    {
        private short PenThickness = 3;
        private const short dotSize = 8;                     // можно поменять

        public override void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas)
        {
            float scaling = size.Width / design.field.cellsx < size.Height / design.field.cellsy ? size.Width / design.field.cellsx : size.Height / design.field.cellsy;
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
            float scaling = size.Width / design.field.cellsx < size.Height / design.field.cellsy ? size.Width / design.field.cellsx : size.Height / design.field.cellsy;
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


        public override void DrawRect(PlaceModel.Design design, PlacementDetail placement, Size size,
                                  System.Drawing.Graphics canvas, int x, int y, int width, int height)
        {
            float scaling = getScaling(size.Width, size.Height, width, height);
            float penBorderDepth = 1;

            Brush br = new SolidBrush(Color.LightGray);
            Pen pen = new Pen(Color.Red, PenThickness);
            Brush brBlack = new SolidBrush(Color.Black);
            // rect for determing intersect with elements that are not fully embraced by the area of drawing
            Rectangle scaleRect = new Rectangle(x, y, width, height);

            canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, width * scaling - 1, height * scaling - 1);


            foreach (var c in design.components)
            {
                if (placement.placed[c])
                {
                    Rectangle currentComponent = new Rectangle(placement.x[c], placement.y[c], c.sizex, c.sizey);

                    if (scaleRect.IntersectsWith(currentComponent)) {

                        canvas.DrawRectangle(pen, (placement.x[c] - x) * scaling,
                        (placement.y[c] - y) * scaling, c.sizex * scaling, c.sizey * scaling);

                        canvas.FillRectangle(br,
                            (placement.x[c] - x) * scaling + penBorderDepth,
                            (placement.y[c] - y) * scaling + penBorderDepth,
                            c.sizex * scaling - penBorderDepth,
                            c.sizey * scaling - penBorderDepth);

                        //canvas.FillEllipse(brBlack, (placement.x[c] - x) * scaling - dotSize / 2, (placement.y[c] - y) * scaling - dotSize / 2, dotSize, dotSize);
                    }
                }
            }
        }


        public override void DrawRect(PlaceModel.Design design, PlacementGlobal placement, Size size,
                                  System.Drawing.Graphics canvas, int x, int y, int width, int height)
        {

            float scaling = getScaling(size.Width, size.Height, width, height);
            float penBorderDepth = 1;

            Brush br = new SolidBrush(Color.LightGray);
            Pen pen = new Pen(Color.Red, 1);
            Brush brBlack = new SolidBrush(Color.Black);
            // rect for determing intersect with elements that are not fully embraced by the area of drawing
            Rectangle scaleRect = new Rectangle(x, y, width, height);

            canvas.DrawRectangle(new Pen(Color.Black, 1), 0, 0, width * scaling - 1, height * scaling - 1);


            foreach (var c in design.components)
            {
                if (placement.placed[c])
                {
                    Rectangle currentComponent = new Rectangle((int)placement.x[c], (int)placement.y[c], c.sizex, c.sizey);

                    if (scaleRect.IntersectsWith(currentComponent))
                    {
                        float xFloat = (float)(placement.x[c] - x) * scaling;
                        float yFloat = (float)(placement.y[c] - y) * scaling;

                        float xSize = c.sizex * scaling;
                        float ySize = c.sizey * scaling;
 

                        canvas.DrawRectangle(pen, (float)(placement.x[c] - x) * scaling,
                        (float)(placement.y[c] - y) * scaling, c.sizex * scaling, c.sizey * scaling);

                        canvas.FillRectangle(br,
                            (float)(placement.x[c] - x) * scaling + penBorderDepth,
                            (float)(placement.y[c] - y) * scaling + penBorderDepth,
                            c.sizex * scaling - penBorderDepth,
                            c.sizey * scaling - penBorderDepth);

                        //canvas.FillEllipse(brBlack, (int)(placement.x[c] - x) * scaling - dotSize / 2, (int)(placement.y[c] - y) * scaling - dotSize / 2, dotSize, dotSize);
                    }
                }
            }
        }

        private float getScaling(float swidth, float sheight, float width, float height) {
            float scaling = swidth / width < sheight / height ? swidth / width : sheight / height;
            return scaling;
        }
    
    }
}