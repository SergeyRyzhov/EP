﻿using System.Drawing;

using PlaceModel;

namespace ChipSynthesys.Draw
{
    public abstract class DrawerBase : IDrawer
    {
        protected int ComponentPenBorderDepth;

        protected Pen ComponentPen;

        protected SolidBrush ComponentBrush;

        protected SolidBrush ComponentMarkBrush;

        protected Pen UnplacedComponentPen;

        protected SolidBrush UnplacedComponentBrush;

        protected int ComponentMarkRadius;

        protected DrawerBase()
        {
            this.ComponentBrush = new SolidBrush(Color.DarkGray);
            this.UnplacedComponentBrush = new SolidBrush(Color.Black);
            this.ComponentMarkBrush = new SolidBrush(Color.Black);
            this.ComponentPenBorderDepth = 1;
            this.ComponentMarkRadius = 8;
            const int componentPenThickness = 8;
            this.ComponentPen = new Pen(Color.Green, componentPenThickness);
            this.UnplacedComponentPen = new Pen(Color.Red, componentPenThickness);
        }

        public abstract void Draw(Design design, PlacementGlobal placement, Size size, Graphics canvas);

        public abstract void Draw(Design design, PlacementDetail placement, Size size, Graphics canvas);

        public abstract void DrawRect(Design design, PlacementGlobal placement, Size size, Graphics canvas, int x, int y, int width, int height);

        public abstract void DrawRect(Design design, PlacementDetail placement, Size size, Graphics canvas, int x, int y, int width, int height);

        protected void MarkComponent(
            Graphics canvas, 
            PlacementGlobal approximate, 
            Component component, 
            float scaling, 
            float offsetOx, 
            float offsetOy)
        {
            float x = (float)(approximate.x[component] - offsetOx) * scaling;
            float y = (float)(approximate.y[component] - offsetOy) * scaling;

            int half = this.ComponentMarkRadius / 2;

            canvas.FillEllipse(ComponentMarkBrush, x - half, y - half, this.ComponentMarkRadius, this.ComponentMarkRadius);
        }

        protected void MarkComponent(
            Graphics canvas, 
            PlacementDetail detail, 
            Component component, 
            float scaling, 
            float offsetOx, 
            float offsetOy)
        {
            float x = (detail.x[component] - offsetOx) * scaling;
            float y = (detail.y[component] - offsetOy) * scaling;

            int half = this.ComponentMarkRadius / 2;

            canvas.FillEllipse(ComponentMarkBrush, x - half, y - half, this.ComponentMarkRadius, this.ComponentMarkRadius);
        }

        protected void DrawComponent(
            Graphics canvas, 
            PlacementGlobal approximate, 
            Component component, 
            float scaling, 
            float offsetOx, 
            float offsetOy)
        {
            float x = (float)(approximate.x[component] - offsetOx) * scaling;
            float y = (float)(approximate.y[component] - offsetOy) * scaling;
            float width = component.sizex * scaling;
            float height = component.sizey * scaling;

            if (approximate.placed[component])
            {
                DrawPlacedComponent(canvas, x, y, width, height);
            }
            else
            {
                DrawUnplacedComponent(canvas, x, y, width, height);
            }
        }

        protected void DrawComponent(
            Graphics canvas, 
            PlacementDetail detail, 
            Component component, 
            float scaling, 
            float offsetOx, 
            float offsetOy)
        {
            float x = (detail.x[component] - offsetOx) * scaling;
            float y = (detail.y[component] - offsetOy) * scaling;
            float width = component.sizex * scaling;
            float height = component.sizey * scaling;

            if (detail.placed[component])
            {
                DrawPlacedComponent(canvas, x, y, width, height);
            }
            else
            {
                DrawUnplacedComponent(canvas, x, y, width, height);
            }
        }

        protected virtual void DrawPlacedComponent(Graphics canvas, float x, float y, float width, float height)
        {
            int border = ComponentPenBorderDepth;

            canvas.DrawRectangle(ComponentPen, x, y, width, height);
            canvas.FillRectangle(ComponentBrush, x + border, y + border, width - border, height - border);
        }
        
        protected virtual void DrawUnplacedComponent(Graphics canvas, float x, float y, float width, float height)
        {
            int border = ComponentPenBorderDepth;

            canvas.DrawRectangle(ComponentPen, x, y, width, height);
            canvas.FillRectangle(ComponentBrush, x + border, y + border, width - border, height - border);
        }
    }
}