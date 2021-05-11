using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Svg.ExtensionMethods
{
    internal static class RectangleFExtensions
    {
        internal static PointF[] GetPoints(this RectangleF rectangle)
        {
            return new[]
            {
                rectangle.Location,
                rectangle.Location + rectangle.Size
            };
        }

        internal static RectangleF Transform(this RectangleF rectangle, Matrix matrix)
        {
            var points = GetPoints(rectangle);
            matrix.TransformPoints(points);
            return points.GetBounds();
        }
    }
}
