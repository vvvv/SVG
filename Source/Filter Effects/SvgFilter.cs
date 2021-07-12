using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Svg.DataTypes;

namespace Svg.FilterEffects
{
    /// <summary>
    /// A filter effect consists of a series of graphics operations that are applied to a given source graphic to produce a modified graphical result.
    /// </summary>
    [SvgElement("filter")]
    public partial class SvgFilter : SvgElement
    {
        [SvgAttribute("filterUnits")]
        public SvgCoordinateUnits FilterUnits
        {
            get { return GetAttribute("filterUnits", false, SvgCoordinateUnits.ObjectBoundingBox); }
            set { Attributes["filterUnits"] = value; }
        }

        [SvgAttribute("primitiveUnits")]
        public SvgCoordinateUnits PrimitiveUnits
        {
            get { return GetAttribute("primitiveUnits", false, SvgCoordinateUnits.UserSpaceOnUse); }
            set { Attributes["primitiveUnits"] = value; }
        }

        /// <summary>
        /// Gets or sets the position where the left point of the filter.
        /// </summary>
        [SvgAttribute("x")]
        public SvgUnit X
        {
            get { return GetAttribute("x", false, new SvgUnit(SvgUnitType.Percentage, -10f)); }
            set { Attributes["x"] = value; }
        }

        /// <summary>
        /// Gets or sets the position where the top point of the filter.
        /// </summary>
        [SvgAttribute("y")]
        public SvgUnit Y
        {
            get { return GetAttribute("y", false, new SvgUnit(SvgUnitType.Percentage, -10f)); }
            set { Attributes["y"] = value; }
        }

        /// <summary>
        /// Gets or sets the width of the resulting filter graphic.
        /// </summary>
        [SvgAttribute("width")]
        public SvgUnit Width
        {
            get { return GetAttribute("width", false, new SvgUnit(SvgUnitType.Percentage, 120f)); }
            set { Attributes["width"] = value; }
        }

        /// <summary>
        /// Gets or sets the height of the resulting filter graphic.
        /// </summary>
        [SvgAttribute("height")]
        public SvgUnit Height
        {
            get { return GetAttribute("height", false, new SvgUnit(SvgUnitType.Percentage, 120f)); }
            set { Attributes["height"] = value; }
        }

        /// <summary>
        /// Gets or sets reference to another filter element within the current document fragment.
        /// </summary>
        [SvgAttribute("href", SvgAttributeAttribute.XLinkNamespace)]
        public Uri Href
        {
            get { return GetAttribute<Uri>("href", false); }
            set { Attributes["href"] = value; }
        }

        /// <summary>
        /// Renders the <see cref="SvgElement"/> and contents to the specified <see cref="ISvgRenderer"/> object.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
        protected override void Render(ISvgRenderer renderer)
        {
            RenderChildren(renderer);
        }

        private Matrix GetTransform(SvgVisualElement element)
        {
            var transformMatrix = new Matrix();
            if (element.Transforms != null)
                using (var matrix = element.Transforms.GetMatrix())
                    transformMatrix.Multiply(matrix);
            return transformMatrix;
        }

        private RectangleF GetPathBounds(SvgVisualElement element, ISvgRenderer renderer, Matrix transform)
        {
            var bounds = element is SvgGroup ? element.Path(renderer).GetBounds() : element.Bounds;
            var pts = new PointF[] { bounds.Location, new PointF(bounds.Right, bounds.Bottom) };
            transform.TransformPoints(pts);

            return new RectangleF(Math.Min(pts[0].X, pts[1].X), Math.Min(pts[0].Y, pts[1].Y),
                                  Math.Abs(pts[0].X - pts[1].X), Math.Abs(pts[0].Y - pts[1].Y));
        }

        public void ApplyFilter(SvgVisualElement element, ISvgRenderer renderer, Action<ISvgRenderer> renderMethod)
        {
            using (var transform = GetTransform(element))
            {
                var bounds = GetPathBounds(element, renderer, transform);
                if (bounds.Width == 0f || bounds.Height == 0f)
                    return;

                var inflate = 0.5f;
                using (var buffer = new ImageBuffer(bounds, inflate, renderer, renderMethod) { Transform = transform })
                {
                    if (this.HasChildren())
                    {
                        foreach (var primitive in Children.OfType<SvgFilterPrimitive>())
                            primitive.Process(buffer);
                    }

                    // Render the final filtered image
                    var bufferImg = buffer.Buffer;
                    var imgDraw = RectangleF.Inflate(bounds, inflate * bounds.Width, inflate * bounds.Height);

                    var prevClip = renderer.GetClip();
                    try
                    {
                        renderer.SetClip(new Region(imgDraw));
                        renderer.DrawImage(bufferImg, imgDraw, new RectangleF(bounds.X, bounds.Y, imgDraw.Width, imgDraw.Height), GraphicsUnit.Pixel);
                    }
                    finally
                    {
                        renderer.SetClip(prevClip);
                    }
                }
            }
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgFilter>();
        }
    }
}
