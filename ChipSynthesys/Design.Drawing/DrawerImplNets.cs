using System.Drawing;

using PlaceModel;

namespace ChipSynthesys.Draw
{
    /// <summary>
    ///     Рисуем компоненты вместе с цепями
    /// </summary>
    public class DrawerImplNets : DrawerImpl
    {
        private Pen netPen;

        public DrawerImplNets()
        {
            netPen = new Pen(Color.FromArgb(64, Color.Black), 1);
        }

        public override void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas)
        {
            base.Draw(design, placement, size, canvas);

            float scaling = GetScaling(size.Width, size.Height, design.field.cellsx, design.field.cellsy);


            for (int i = 0; i < design.nets.Length; i++)
            {
                double drawnNetsY = 0.0;
                if (design.nets[i].items.Length == 0)
                {
                    continue;
                }

                float xMin = design.field.cellsx * scaling;
                float xMax = 0;


                foreach (Component comp in design.nets[i].items)
                {
                    if (placement.x[comp] * scaling + comp.sizex * scaling / 2 < xMin)
                    {
                        xMin = (float)placement.x[comp] * scaling + comp.sizex * scaling / 2;
                    }

                    if (placement.x[comp] * scaling + comp.sizex * scaling / 2 > xMax)
                    {
                        xMax = (float)placement.x[comp] * scaling + comp.sizex * scaling / 2;
                    }

                    drawnNetsY += placement.y[comp];
                }

                drawnNetsY = drawnNetsY / design.nets[i].items.Length * scaling;

                canvas.DrawLine(netPen, xMin, (int)drawnNetsY, xMax, (int)drawnNetsY);

                foreach (Component comp in design.nets[i].items)
                {
                    canvas.DrawLine(
                        netPen,
                        (float)placement.x[comp] * scaling + (float)comp.sizex / 2 * scaling,
                        (float)placement.y[comp] * scaling + (float)comp.sizey / 2 * scaling,
                        (float)placement.x[comp] * scaling + (float)comp.sizex / 2 * scaling,
                        (float)drawnNetsY);

                    MarkComponent(canvas, placement, comp, scaling, 0, 0);
                }
            }
        }

        public override void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas)
        {
            base.Draw(design, placement, size, canvas);

            float scaling = GetScaling(size.Width, size.Height, design.field.cellsx, design.field.cellsy);


            for (int i = 0; i < design.nets.Length; i++)
            {
                double drawnNetsY = 0.0;
                if (design.nets[i].items.Length == 0)
                {
                    continue;
                }

                float xMin = design.field.cellsx * scaling;
                float xMax = 0;


                foreach (Component comp in design.nets[i].items)
                {
                    if (placement.x[comp] * scaling + comp.sizex * scaling / 2 < xMin)
                    {
                        xMin = placement.x[comp] * scaling + comp.sizex * scaling / 2;
                    }

                    if (placement.x[comp] * scaling + comp.sizex * scaling / 2 > xMax)
                    {
                        xMax = placement.x[comp] * scaling + comp.sizex * scaling / 2;
                    }

                    drawnNetsY += placement.y[comp];
                }

                drawnNetsY = drawnNetsY / design.nets[i].items.Length * scaling;

                canvas.DrawLine(netPen, xMin, (int)drawnNetsY, xMax, (int)drawnNetsY);

                foreach (Component comp in design.nets[i].items)
                {
                    canvas.DrawLine(
                        netPen,
                        placement.x[comp] * scaling + (float)comp.sizex / 2 * scaling,
                        placement.y[comp] * scaling + (float)comp.sizey / 2 * scaling,
                        placement.x[comp] * scaling + (float)comp.sizex / 2 * scaling,
                        (float)drawnNetsY);

                    MarkComponent(canvas, placement, comp, scaling, 0, 0);
                }
            }
        }

    }
}