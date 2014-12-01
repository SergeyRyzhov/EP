using System;
using System.Drawing;

using ChipSynthesys.Common;

using PlaceModel;

namespace ChipSynthesys.Draw
{
    public static class DesignExtensions
    {
        public static Size AdjustSize(this Design design)
        {
            float width = design.field.cellsx * TestsConstants.Scale;
            float height = design.field.cellsy * TestsConstants.Scale;

            float scaleHeight = height < TestsConstants.ImageHeight ? TestsConstants.ImageHeight / height : 1;
            float scaleWidth = width < TestsConstants.ImageWidth ? TestsConstants.ImageWidth / width : 1;

            float scale = Math.Max(scaleHeight, scaleWidth);

            height *= scale;
            width *= scale;

            return new Size((int)width, (int)height);
        }
    }
}
