using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    /// <summary>
    /// An <see cref="SvgFragment"/> represents an SVG fragment that can be the root element or an embedded fragment of an SVG document.
    /// </summary>
    [SvgElement("svg")]
    public partial class SvgFragment : SvgElement, ISvgViewPort, ISvgBoundable
    {
        private SvgUnit _x = 0f;
        private SvgUnit _y = 0f;

        /// <summary>
        /// Gets the SVG namespace string.
        /// </summary>
        public static readonly Uri Namespace = new Uri(SvgNamespaces.SvgNamespace);

        PointF ISvgBoundable.Location
        {
            get { return PointF.Empty; }
        }

        SizeF ISvgBoundable.Size
        {
            get
            {
                // Prevent stack overflow due to mutually recursive call.
                if (Width.Type == SvgUnitType.Percentage || Height.Type == SvgUnitType.Percentage)
                    return new SizeF();
                return GetDimensions();
            }
        }

        RectangleF ISvgBoundable.Bounds
        {
            get { return new RectangleF(((ISvgBoundable)this).Location, ((ISvgBoundable)this).Size); }
        }

        /// <summary>
        /// Gets or sets the position where the left point of the svg should start.
        /// </summary>
        [SvgAttribute("x")]
        public SvgUnit X
        {
            get { return _x; }
            set
            {
                if (_x != value)
                    _x = value;
                Attributes["x"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the position where the top point of the svg should start.
        /// </summary>
        [SvgAttribute("y")]
        public SvgUnit Y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                    _y = value;
                Attributes["y"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the fragment.
        /// </summary>
        /// <value>The width.</value>
        [SvgAttribute("width")]
        public SvgUnit Width
        {
            get { return GetAttribute("width", false, new SvgUnit(SvgUnitType.Percentage, 100f)); }
            set { Attributes["width"] = value; }
        }

        /// <summary>
        /// Gets or sets the height of the fragment.
        /// </summary>
        /// <value>The height.</value>
        [SvgAttribute("height")]
        public SvgUnit Height
        {
            get { return GetAttribute("height", false, new SvgUnit(SvgUnitType.Percentage, 100f)); }
            set { Attributes["height"] = value; }
        }

        [SvgAttribute("overflow")]
        public virtual SvgOverflow Overflow
        {
            get { return GetAttribute("overflow", false, SvgOverflow.Hidden); }
            set { Attributes["overflow"] = value; }
        }

        /// <summary>
        /// Gets or sets the viewport of the element.
        /// </summary>
        /// <value></value>
        [SvgAttribute("viewBox")]
        public SvgViewBox ViewBox
        {
            get { return GetAttribute("viewBox", false, SvgViewBox.Empty); }
            set { Attributes["viewBox"] = value; }
        }

        /// <summary>
        /// Gets or sets the aspect of the viewport.
        /// </summary>
        /// <value></value>
        [SvgAttribute("preserveAspectRatio")]
        public SvgAspectRatio AspectRatio
        {
            get { return GetAttribute("preserveAspectRatio", false, new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid)); }
            set { Attributes["preserveAspectRatio"] = value; }
        }

        /// <summary>
        /// Refers to the size of the font from baseline to baseline when multiple lines of text are set solid in a multiline layout environment.
        /// </summary>
        [SvgAttribute("font-size")]
        public override SvgUnit FontSize
        {
            get { return GetAttribute("font-size", true, SvgUnit.Empty); }
            set { Attributes["font-size"] = value; }
        }

        /// <summary>
        /// Indicates which font family is to be used to render the text.
        /// </summary>
        [SvgAttribute("font-family")]
        public override string FontFamily
        {
            get { return GetAttribute<string>("font-family", true); }
            set { Attributes["font-family"] = value; }
        }

        public override XmlSpaceHandling SpaceHandling
        {
            get { return GetAttribute("space", true, XmlSpaceHandling.Default); }
            set { base.SpaceHandling = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Applies the required transforms to <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
        protected internal override bool PushTransforms(ISvgRenderer renderer)
        {
            if (!base.PushTransforms(renderer))
                return false;
            ViewBox.AddViewBoxTransform(AspectRatio, renderer, this);
            return true;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            switch (this.Overflow)
            {
                case SvgOverflow.Auto:
                case SvgOverflow.Visible:
                case SvgOverflow.Inherit:
                    base.Render(renderer);
                    break;
                default:
                    var prevClip = renderer.GetClip();
                    try
                    {
                        var size = this.Parent == null ? renderer.GetBoundable().Bounds.Size : GetDimensions();
                        var clip = new RectangleF(this.X.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this),
                                                  this.Y.ToDeviceValue(renderer, UnitRenderingType.Vertical, this),
                                                  size.Width, size.Height);
                        renderer.SetClip(new Region(clip), CombineMode.Intersect);
                        try
                        {
                            renderer.SetBoundable(new GenericBoundable(clip));
                            base.Render(renderer);
                        }
                        finally
                        {
                            renderer.PopBoundable();
                        }
                    }
                    finally
                    {
                        renderer.SetClip(prevClip, CombineMode.Replace);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the <see cref="GraphicsPath"/> for this element.
        /// </summary>
        /// <value></value>
        public GraphicsPath Path
        {
            get
            {
                var path = new GraphicsPath();

                AddPaths(this, path);

                return path;
            }
        }

        /// <summary>
        /// Gets the bounds of the svg element.
        /// </summary>
        /// <value>The bounds.</value>
        public RectangleF Bounds
        {
            get
            {
                var bounds = new RectangleF();
                foreach (var child in this.Children)
                {
                    RectangleF childBounds = new RectangleF();
                    if (child is SvgFragment)
                    {
                        childBounds = ((SvgFragment)child).Bounds;
                        childBounds.Offset(((SvgFragment)child).X, ((SvgFragment)child).Y);
                    }
                    else if (child is SvgVisualElement)
                    {
                        childBounds = ((SvgVisualElement)child).Bounds;
                    }

                    if (!childBounds.IsEmpty)
                    {
                        if (bounds.IsEmpty)
                        {
                            bounds = childBounds;
                        }
                        else
                        {
                            bounds = RectangleF.Union(bounds, childBounds);
                        }
                    }
                }

                return TransformedBounds(bounds);
            }
        }

        public SizeF GetDimensions()
        {
            float w, h;
            var isWidthperc = Width.Type == SvgUnitType.Percentage;
            var isHeightperc = Height.Type == SvgUnitType.Percentage;

            var bounds = new RectangleF();
            if (isWidthperc || isHeightperc)
            {
                if (ViewBox.Width > 0 && ViewBox.Height > 0)
                {
                    bounds = new RectangleF(ViewBox.MinX, ViewBox.MinY, ViewBox.Width, ViewBox.Height);
                }
                else
                {
                    bounds = this.Bounds; // do just one call to the recursive bounds property
                }
            }

            if (isWidthperc)
            {
                w = (bounds.Width + bounds.X) * (Width.Value * 0.01f);
            }
            else
            {
                w = Width.ToDeviceValue(null, UnitRenderingType.Horizontal, this);
            }
            if (isHeightperc)
            {
                h = (bounds.Height + bounds.Y) * (Height.Value * 0.01f);
            }
            else
            {
                h = Height.ToDeviceValue(null, UnitRenderingType.Vertical, this);
            }

            return new SizeF(w, h);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgFragment>();
        }

        public override SvgElement DeepCopy<T>()
        {
            var newObj = base.DeepCopy<T>() as SvgFragment;

            newObj._x = _x;
            newObj._y = _y;
            return newObj;
        }
    }
}
