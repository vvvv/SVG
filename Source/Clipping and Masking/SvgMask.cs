using System;
using System.Drawing;

namespace Svg
{
    /// <summary>
    /// Defines an alpha mask for compositing the current object into the background.
    /// </summary>
    [SvgElement("mask")]
    public partial class SvgMask : SvgElement
    {
        /// <summary>
        /// Defines the coordinate system for attributes <see cref="X"/>, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height"/>.
        /// </summary>
        [SvgAttribute("maskUnits")]
        public SvgCoordinateUnits MaskUnits
        {
            get { return GetAttribute("maskUnits", false, SvgCoordinateUnits.ObjectBoundingBox); }
            set { Attributes["maskUnits"] = value; }
        }

        /// <summary>
        /// Defines the coordinate system for the contents of the mask.
        /// </summary>
        [SvgAttribute("maskContentUnits")]
        public SvgCoordinateUnits MaskContentUnits
        {
            get { return GetAttribute("maskContentUnits", false, SvgCoordinateUnits.UserSpaceOnUse); }
            set { Attributes["maskContentUnits"] = value; }
        }

        /// <summary>
        /// The x-axis coordinate of one corner of the rectangle for the largest possible offscreen buffer.
        /// </summary>
        [SvgAttribute("x")]
        public SvgUnit X
        {
            get { return GetAttribute("x", false, new SvgUnit(SvgUnitType.Percentage, -10f)); }
            set { Attributes["x"] = value; }
        }

        /// <summary>
        /// The y-axis coordinate of one corner of the rectangle for the largest possible offscreen buffer.
        /// </summary>
        [SvgAttribute("y")]
        public SvgUnit Y
        {
            get { return GetAttribute("y", false, new SvgUnit(SvgUnitType.Percentage, -10f)); }
            set { Attributes["y"] = value; }
        }

        /// <summary>
        /// The width of the largest possible offscreen buffer.
        /// </summary>
        [SvgAttribute("width")]
        public SvgUnit Width
        {
            get { return GetAttribute("width", false, new SvgUnit(SvgUnitType.Percentage, 120f)); }
            set { Attributes["width"] = value; }
        }

        /// <summary>
        /// The height of the largest possible offscreen buffer.
        /// </summary>
        [SvgAttribute("height")]
        public SvgUnit Height
        {
            get { return GetAttribute("height", false, new SvgUnit(SvgUnitType.Percentage, 120f)); }
            set { Attributes["height"] = value; }
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgMask>();
        }

        /// <summary>
        /// Renders the mask to a bitmap.
        /// </summary>
        /// <param name="renderer">The current renderer.</param>
        /// <returns>A <see cref="Bitmap"/> object.</returns>
        public Bitmap RenderMask(ISvgRenderer renderer)
        {
            var boundable = renderer.GetBoundable();

            var maskBitmap = new Bitmap((int)Math.Round(renderer.RenderSize.Width), (int)Math.Round(renderer.RenderSize.Height));

            var graphics = Graphics.FromImage(maskBitmap);
            graphics.Transform = renderer.Transform.Clone();
            graphics.Clear(System.Drawing.Color.Black);

            using (var maskRenderer = SvgRenderer.FromGraphics(graphics, renderer.RenderSize))
            {
                maskRenderer.SetBoundable(boundable);

                RenderElement(maskRenderer);
            }

            return maskBitmap;
        }
    }
}
