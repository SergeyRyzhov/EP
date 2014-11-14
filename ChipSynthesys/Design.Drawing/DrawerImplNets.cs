using System;
using System.Drawing;
using System.Linq;

using PlaceModel;

namespace ChipSynthesys.Draw
{
    /// <summary>
    ///     Рисуем компоненты вместе с цепями
    /// </summary>
    public class DrawerImplNets : DrawerImpl
    {
        protected Pen NetPen;

        public DrawerImplNets()
        {
            this.NetPen = new Pen(Color.FromArgb(48, Color.DarkBlue), 1.25f);
        }

        public override void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas)
        {
            base.Draw(design, placement, size, canvas);

            float scaling = GetScaling(size.Width, size.Height, design.field.cellsx, design.field.cellsy);

            foreach (Net net in design.nets)
            {
                this.DrawNet(design, c => (float)placement.x[c], c => (float)placement.y[c], canvas, net, scaling);
            }
        }

        public override void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas)
        {
            base.Draw(design, placement, size, canvas);

            float scaling = GetScaling(size.Width, size.Height, design.field.cellsx, design.field.cellsy);

            foreach (Net net in design.nets)
            {
                this.DrawNet(design, c => placement.x[c], c => placement.y[c], canvas, net, scaling);
            }
        }

        protected virtual void DrawNet(Design design, Func<Component, float> xGetter, Func<Component, float> yGetter, Graphics canvas, Net net, float scaling)
        {
            const bool drawSpline = true;
            if (drawSpline)
            {
                const float beta = 0.75f;
                const float step = 0.01f;
                canvas.DrawBSpline(
                    NetPen,
                    net.items.Select(c => new PointF(xGetter(c), yGetter(c))).ToArray(),
                    beta,
                    step);
                return;
            }

            double averageOy = 0.0;
            double averageOx = 0.0;
            if (net.items.Length == 0)
            {
                return;
            }

            float xMin = design.field.cellsx * scaling;
            float xMax = design.field.beginx;

            foreach (Component comp in net.items)
            {
                int halfSize = comp.sizex / 2;
                float position = (xGetter(comp) + halfSize) * scaling;
                if (position < xMin)
                {
                    xMin = position;
                }

                if (position > xMax)
                {
                    xMax = position;
                }

                averageOy += yGetter(comp);
                averageOx += xGetter(comp);
            }

            averageOy = averageOy / net.items.Length * scaling;
            averageOx = averageOx / net.items.Length * scaling;

            canvas.DrawLine(this.NetPen, xMin, (int)averageOy, xMax, (int)averageOy);

            foreach (Component comp in net.items)
            {
                int halfSizeOx = comp.sizex / 2;
                int halfSizeOy = comp.sizey / 2;

                float x = (xGetter(comp) + halfSizeOx) * scaling;
                float y = (yGetter(comp) + halfSizeOy) * scaling;

                canvas.DrawLine(this.NetPen, x, y, (float)x, (float)averageOy);

                this.MarkComponent(canvas, xGetter, yGetter, comp, scaling, 0, 0);
            }
        }

        protected void MarkComponent(
            Graphics canvas,
            Func<Component, float> xGetter,
            Func<Component, float> yGetter,
            Component component,
            float scaling,
            float offsetOx,
            float offsetOy)
        {
            float x = (xGetter(component) - offsetOx) * scaling;
            float y = (yGetter(component) - offsetOy) * scaling;

            int half = this.ComponentMarkRadius / 2;

            canvas.FillEllipse(
                this.ComponentMarkBrush,
                x - half,
                y - half,
                this.ComponentMarkRadius,
                this.ComponentMarkRadius);
        }
    }
}