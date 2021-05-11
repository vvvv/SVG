using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Svg.ExtensionMethods
{
    internal static class PointFExtensions
    {
        internal static RectangleF GetBounds(this IEnumerable<PointF> points)
        {
            var minX = points.Min(point => point.X);
            var maxX = points.Max(point => point.X);
            var minY = points.Min(point => point.Y);
            var maxY = points.Max(point => point.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
