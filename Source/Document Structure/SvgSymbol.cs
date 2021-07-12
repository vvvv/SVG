using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    /// <summary>
    /// An element used to group SVG shapes.
    /// </summary>
    [SvgElement("symbol")]
    public partial class SvgSymbol : SvgVisualElement
    {
        /// <summary>
        /// Gets or sets the viewport of the element.
        /// </summary>
        /// <value></value>
        [SvgAttribute("viewBox")]
        public SvgViewBox ViewBox
        {
            get { return GetAttribute<SvgViewBox>("viewBox", false); }
            set { Attributes["viewBox"] = value; }
        }

        /// <summary>
        /// Gets or sets the aspect of the viewport.
        /// </summary>
        /// <value></value>
        [SvgAttribute("preserveAspectRatio")]
        public SvgAspectRatio AspectRatio
        {
            get { return GetAttribute<SvgAspectRatio>("preserveAspectRatio", false); }
            set { Attributes["preserveAspectRatio"] = value; }
        }

        /// <summary>
        /// Gets the <see cref="GraphicsPath"/> for this element.
        /// </summary>
        /// <value></value>
        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            return GetPaths(this, renderer);
        }

        /// <summary>
        /// Gets the bounds of the element.
        /// </summary>
        /// <value>The bounds.</value>
        public override RectangleF Bounds
        {
            get
            {
                var r = new RectangleF();
                if (this.HasChildren())
                {
                    foreach (var c in this.Children)
                    {
                        if (c is SvgVisualElement)
                        {
                            // First it should check if rectangle is empty or it will return the wrong Bounds.
                            // This is because when the Rectangle is Empty, the Union method adds as if the first values where X=0, Y=0
                            if (r.IsEmpty)
                            {
                                r = ((SvgVisualElement)c).Bounds;
                            }
                            else
                            {
                                var childBounds = ((SvgVisualElement)c).Bounds;
                                if (!childBounds.IsEmpty)
                                {
                                    r = RectangleF.Union(r, childBounds);
                                }
                            }
                        }
                    }
                }

                return TransformedBounds(r);
            }
        }

        protected override bool Renderable { get { return false; } }

        /// <summary>
        /// Applies the required transforms to <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
        protected internal override bool PushTransforms(ISvgRenderer renderer)
        {
            if (!base.PushTransforms(renderer))
                return false;
            ViewBox.AddViewBoxTransform(AspectRatio, renderer, null);
            return true;
        }

        // Only render if the parent is set to a Use element
        protected override void Render(ISvgRenderer renderer)
        {
            if (_parent is SvgUse) base.Render(renderer);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgSymbol>();
        }
    }
}

namespace Svg.Document_Structure
{
    [Obsolete("Use Svg.SvgSymbol.")]
    [SvgElement("")]
    public partial class SvgSymbol : Svg.SvgSymbol
    {
        public SvgSymbol()
        {
            typeof(SvgElement)
                .GetField("_elementName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(this, "symbol");
        }
    }
}
