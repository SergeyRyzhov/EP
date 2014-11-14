using System.Drawing;

using PlaceModel;

namespace ChipSynthesys.Draw
{
    public class DrawerImpl : DrawerBase
    {
        public override void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas)
        {
            float scaling = GetScaling(size.Width, size.Height, design.field.cellsx, design.field.cellsy);

            var scaleRect = new Rectangle(
                design.field.beginx,
                design.field.beginy,
                design.field.cellsx,
                design.field.cellsy);

            foreach (var c in design.components)
            {
                var currentComponent = new Rectangle((int)placement.x[c], (int)placement.y[c], c.sizex, c.sizey);

                if (scaleRect.IntersectsWith(currentComponent))
                {
                    DrawComponent(canvas, placement, c, scaling, design.field.beginx, design.field.beginy);
                }
            }
        }

        public override void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas)
        {
            float scaling = GetScaling(size.Width, size.Height, design.field.cellsx, design.field.cellsy);

            var scaleRect = new Rectangle(
                design.field.beginx,
                design.field.beginy,
                design.field.cellsx,
                design.field.cellsy);

            foreach (var c in design.components)
            {
                var currentComponent = new Rectangle(placement.x[c], placement.y[c], c.sizex, c.sizey);

                if (scaleRect.IntersectsWith(currentComponent))
                {
                    DrawComponent(canvas, placement, c, scaling, design.field.beginx, design.field.beginy);
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
            float scaling = GetScaling(size.Width, size.Height, width, height);

            var scaleRect = new Rectangle(x, y, width, height);

            foreach (var c in design.components)
            {
                var currentComponent = new Rectangle(placement.x[c], placement.y[c], c.sizex, c.sizey);

                if (scaleRect.IntersectsWith(currentComponent))
                {
                    DrawComponent(canvas, placement, c, scaling, x, y);
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
            float scaling = GetScaling(size.Width, size.Height, width, height);

            var scaleRect = new Rectangle(x, y, width, height);

            foreach (var c in design.components)
            {
                var currentComponent = new Rectangle((int)placement.x[c], (int)placement.y[c], c.sizex, c.sizey);

                if (scaleRect.IntersectsWith(currentComponent))
                {
                    DrawComponent(canvas, placement, c, scaling, x, y);
                }
            }
        }

        protected static float GetScaling(float fieldWidth, float fieldHeight, float width, float height)
        {
            float scaling = fieldWidth / width < fieldHeight / height ? fieldWidth / width : fieldHeight / height;
            return scaling;
        }
    }
}