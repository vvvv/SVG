using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Svg.Transforms;

namespace Svg
{
    /// <summary>
    /// A pattern is used to fill or stroke an object using a pre-defined graphic object which can be replicated ("tiled") at fixed intervals in x and y to cover the areas to be painted.
    /// </summary>
    [SvgElement("pattern")]
    public partial class SvgPatternServer : SvgPaintServer, ISvgViewPort
    {
        private SvgUnit _x = SvgUnit.None;
        private SvgUnit _y = SvgUnit.None;
        private SvgUnit _width = SvgUnit.None;
        private SvgUnit _height = SvgUnit.None;
        private SvgCoordinateUnits? _patternUnits;
        private SvgCoordinateUnits? _patternContentUnits;
        private SvgViewBox _viewBox;

        /// <summary>
        /// Gets or sets the X-axis location of the pattern.
        /// </summary>
        [SvgAttribute("x")]
        public SvgUnit X
        {
            get { return _x; }
            set { _x = value; Attributes["x"] = value; }
        }

        /// <summary>
        /// Gets or sets the Y-axis location of the pattern.
        /// </summary>
        [SvgAttribute("y")]
        public SvgUnit Y
        {
            get { return _y; }
            set { _y = value; Attributes["y"] = value; }
        }

        /// <summary>
        /// Gets or sets the width of the pattern.
        /// </summary>
        [SvgAttribute("width")]
        public SvgUnit Width
        {
            get { return _width; }
            set { _width = value; Attributes["width"] = value; }
        }

        /// <summary>
        /// Gets or sets the height of the pattern.
        /// </summary>
        [SvgAttribute("height")]
        public SvgUnit Height
        {
            get { return _height; }
            set { _height = value; Attributes["height"] = value; }
        }

        /// <summary>
        /// Gets or sets the width of the pattern.
        /// </summary>
        [SvgAttribute("patternUnits")]
        public SvgCoordinateUnits PatternUnits
        {
            get { return _patternUnits ?? SvgCoordinateUnits.ObjectBoundingBox; }
            set { _patternUnits = value; Attributes["patternUnits"] = value; }
        }

        /// <summary>
        /// Gets or sets the width of the pattern.
        /// </summary>
        [SvgAttribute("patternContentUnits")]
        public SvgCoordinateUnits PatternContentUnits
        {
            get { return _patternContentUnits ?? SvgCoordinateUnits.UserSpaceOnUse; }
            set { _patternContentUnits = value; Attributes["patternContentUnits"] = value; }
        }

        /// <summary>
        /// Specifies a supplemental transformation which is applied on top of any
        /// transformations necessary to create a new pattern coordinate system.
        /// </summary>
        [SvgAttribute("viewBox")]
        public SvgViewBox ViewBox
        {
            get { return _viewBox; }
            set { _viewBox = value; Attributes["viewBox"] = value; }
        }

        /// <summary>
        /// Gets or sets another gradient fill from which to inherit the stops from.
        /// </summary>
        [SvgAttribute("href", SvgAttributeAttribute.XLinkNamespace)]
        public SvgDeferredPaintServer InheritGradient
        {
            get { return GetAttribute<SvgDeferredPaintServer>("href", false); }
            set { Attributes["href"] = value; }
        }

        [SvgAttribute("overflow")]
        public SvgOverflow Overflow
        {
            get { return GetAttribute("overflow", false, SvgOverflow.Hidden); }
            set { Attributes["overflow"] = value; }
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

        [SvgAttribute("patternTransform")]
        public SvgTransformCollection PatternTransform
        {
            get { return GetAttribute<SvgTransformCollection>("patternTransform", false); }
            set { Attributes["patternTransform"] = value; }
        }

        private Matrix EffectivePatternTransform
        {
            get
            {
                var transform = new Matrix();
                if (PatternTransform != null)
                    using (var matrix = PatternTransform.GetMatrix())
                        transform.Multiply(matrix);

                return transform;
            }
        }

        /// <summary>
        /// Gets a <see cref="Brush"/> representing the current paint server.
        /// </summary>
        /// <param name="renderingElement">The owner <see cref="SvgVisualElement"/>.</param>
        /// <param name="renderer">The renderer object.</param>
        /// <param name="opacity">The opacity of the brush.</param>
        /// <param name="forStroke">Not used.</param>
        public override Brush GetBrush(SvgVisualElement renderingElement, ISvgRenderer renderer, float opacity, bool forStroke = false)
        {
            var chain = new List<SvgPatternServer>();

            var curr = this;
            do
            {
                chain.Add(curr);
                curr = SvgDeferredPaintServer.TryGet<SvgPatternServer>(curr.InheritGradient, renderingElement);
            } while (curr != null);

            var firstChildren = chain.Where(p => p.HasChildren()).FirstOrDefault();
            if (firstChildren == null)
                return null;
            var firstX = chain.Where(p => p.X != SvgUnit.None).FirstOrDefault();
            var firstY = chain.Where(p => p.Y != SvgUnit.None).FirstOrDefault();
            var firstWidth = chain.Where(p => p.Width != SvgUnit.None).FirstOrDefault();
            var firstHeight = chain.Where(p => p.Height != SvgUnit.None).FirstOrDefault();
            if (firstWidth == null || firstHeight == null)
                return null;
            var firstPatternUnit = chain.Where(p => p._patternUnits.HasValue).FirstOrDefault();
            var firstPatternContentUnit = chain.Where(p => p._patternContentUnits.HasValue).FirstOrDefault();
            var firstViewBox = chain.Where(p => p.ViewBox != SvgViewBox.Empty).FirstOrDefault();

            var xUnit = firstX == null ? new SvgUnit(0f) : firstX.X;
            var yUnit = firstY == null ? new SvgUnit(0f) : firstY.Y;
            var widthUnit = firstWidth.Width;
            var heightUnit = firstHeight.Height;

            var patternUnits = firstPatternUnit == null ? SvgCoordinateUnits.ObjectBoundingBox : firstPatternUnit.PatternUnits;
            var patternContentUnits = firstPatternContentUnit == null ? SvgCoordinateUnits.UserSpaceOnUse : firstPatternContentUnit.PatternContentUnits;
            var viewBox = firstViewBox == null ? SvgViewBox.Empty : firstViewBox.ViewBox;

            var isPatternObjectBoundingBox = patternUnits == SvgCoordinateUnits.ObjectBoundingBox;
            try
            {
                if (isPatternObjectBoundingBox)
                    renderer.SetBoundable(renderingElement);

                var x = xUnit.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this);
                var y = yUnit.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
                var width = widthUnit.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this);
                var height = heightUnit.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);

                if (isPatternObjectBoundingBox)
                {
                    var bounds = renderer.GetBoundable().Bounds;    // Boundary without stroke is expect...

                    if (xUnit.Type != SvgUnitType.Percentage)
                        x *= bounds.Width;
                    if (yUnit.Type != SvgUnitType.Percentage)
                        y *= bounds.Height;
                    if (widthUnit.Type != SvgUnitType.Percentage)
                        width *= bounds.Width;
                    if (heightUnit.Type != SvgUnitType.Percentage)
                        height *= bounds.Height;
                    x += bounds.X;
                    y += bounds.Y;
                }

                if (width <= 0f || height <= 0f)
                    return null;

                var tile = new Bitmap((int)Math.Ceiling(width), (int)Math.Ceiling(height));
                using (var tileRenderer = SvgRenderer.FromImage(tile))
                {
                    tileRenderer.SetBoundable(renderingElement);
                    if (viewBox != SvgViewBox.Empty)
                    {
                        var bounds = tileRenderer.GetBoundable().Bounds;
                        tileRenderer.ScaleTransform(width / viewBox.Width, height / viewBox.Height);
                    }
                    else if (patternContentUnits == SvgCoordinateUnits.ObjectBoundingBox)
                    {
                        var bounds = tileRenderer.GetBoundable().Bounds;
                        tileRenderer.ScaleTransform(bounds.Width, bounds.Height);
                    }

                    if (firstChildren.HasChildren())
                    {
                        foreach (var child in firstChildren.Children)
                            child.RenderElement(tileRenderer);
                    }
                }

                using (var transform = EffectivePatternTransform)
                {
                    var textureBrush = new TextureBrush(tile, new RectangleF(0f, 0f, width, height))
                    {
                        Transform = transform
                    };
                    textureBrush.TranslateTransform(x, y);
                    return textureBrush;
                }
            }
            finally
            {
                if (isPatternObjectBoundingBox)
                    renderer.PopBoundable();
            }
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgPatternServer>();
        }

        public override SvgElement DeepCopy<T>()
        {
            var newObj = base.DeepCopy<T>() as SvgPatternServer;

            newObj._x = _x;
            newObj._y = _y;
            newObj._width = _width;
            newObj._height = _height;
            newObj._patternUnits = _patternUnits;
            newObj._patternContentUnits = _patternContentUnits;
            newObj._viewBox = _viewBox;
            return newObj;
        }
    }
}
