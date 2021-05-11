using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using Svg.ExtensionMethods;
using Svg.FilterEffects;

namespace Svg
{
    /// <summary>
    /// The class that all SVG elements should derive from when they are to be rendered.
    /// </summary>
    public abstract partial class SvgVisualElement : SvgElement, ISvgBoundable, ISvgStylable, ISvgClipable
    {
        private bool? _requiresSmoothRendering;
        private Region _previousClip;
        private Bitmap _previousMask;

        /// <summary>
        /// Gets the <see cref="GraphicsPath"/> for this element.
        /// </summary>
        public abstract GraphicsPath Path(ISvgRenderer renderer);

        PointF ISvgBoundable.Location
        {
            get
            {
                return Bounds.Location;
            }
        }

        SizeF ISvgBoundable.Size
        {
            get
            {
                return Bounds.Size;
            }
        }

        /// <summary>
        /// Gets the bounds of the element.
        /// </summary>
        /// <value>The bounds.</value>
        public abstract RectangleF Bounds { get; }

        /// <summary>
        /// Gets the associated <see cref="SvgClipPath"/> if one has been specified.
        /// </summary>
        [SvgAttribute("clip")]
        public virtual string Clip
        {
            get { return GetAttribute("clip", true, "auto"); }
            set { Attributes["clip"] = value; }
        }

        /// <summary>
        /// Gets the associated <see cref="SvgClipPath"/> if one has been specified.
        /// </summary>
        [SvgAttribute("clip-path")]
        public virtual Uri ClipPath
        {
            get { return GetAttribute<Uri>("clip-path", false); }
            set { Attributes["clip-path"] = value; }
        }

        /// <summary>
        /// Gets the associated <see cref="SvgMask"/> if one has been specified.
        /// </summary>
        [SvgAttribute("mask")]
        public virtual Uri Mask
        {
            get { return GetAttribute<Uri>("mask", false); }
            set { Attributes["mask"] = value; }
        }

        /// <summary>
        /// Gets or sets the algorithm which is to be used to determine the clipping region.
        /// </summary>
        [SvgAttribute("clip-rule")]
        public SvgClipRule ClipRule
        {
            get { return GetAttribute("clip-rule", true, SvgClipRule.NonZero); }
            set { Attributes["clip-rule"] = value; }
        }

        /// <summary>
        /// Gets the associated <see cref="SvgFilter"/> if one has been specified.
        /// </summary>
        [SvgAttribute("filter")]
        public virtual Uri Filter
        {
            get { return GetAttribute<Uri>("filter", false); }
            set { Attributes["filter"] = value; }
        }

        /// <summary>
        /// Gets or sets a value to determine if anti-aliasing should occur when the element is being rendered.
        /// </summary>
        protected virtual bool RequiresSmoothRendering
        {
            get
            {
                if (_requiresSmoothRendering == null)
                    _requiresSmoothRendering = ConvertShapeRendering2AntiAlias(ShapeRendering);

                return _requiresSmoothRendering.Value;
            }
        }

        private bool ConvertShapeRendering2AntiAlias(SvgShapeRendering shapeRendering)
        {
            switch (shapeRendering)
            {
                case SvgShapeRendering.OptimizeSpeed:
                case SvgShapeRendering.CrispEdges:
                case SvgShapeRendering.GeometricPrecision:
                    return false;
                default:
                    // SvgShapeRendering.Auto
                    // SvgShapeRendering.Inherit
                    return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgVisualElement"/> class.
        /// </summary>
        public SvgVisualElement()
        {
            this.IsPathDirty = true;
        }

        protected virtual bool Renderable { get { return true; } }

        /// <summary>
        /// Renders the <see cref="SvgElement"/> and contents to the specified <see cref="Graphics"/> object.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
        protected override void Render(ISvgRenderer renderer)
        {
            if (Visible && Displayable && (!Renderable || Path(renderer) != null))
                RenderInternal(renderer, true);
        }

        private void RenderInternal(ISvgRenderer renderer, bool renderFilter)
        {
            if (!(renderFilter && RenderFilter(renderer)))
            {
                var opacity = FixOpacityValue(Opacity);
                if (opacity == 1f)
                    if (Renderable)
                        RenderInternal(renderer, RenderFillAndStroke);
                    else
                        RenderInternal(renderer, RenderChildren);
                else
                {
                    IsPathDirty = true;
                    var bounds = Renderable ? Bounds : Path(null).GetBounds();
                    IsPathDirty = true;

                    if (bounds.Width > 0f && bounds.Height > 0f)
                    {
                        var scaleX = 1f;
                        var scaleY = 1f;
                        using (var transform = renderer.Transform)
                        {
                            scaleX = Math.Max(scaleX, Math.Abs(transform.Elements[0]));
                            scaleY = Math.Max(scaleY, Math.Abs(transform.Elements[3]));
                        }

                        using (var canvas = new Bitmap((int)Math.Ceiling(bounds.Width * scaleX), (int)Math.Ceiling(bounds.Height * scaleY)))
                        {
                            using (var canvasRenderer = SvgRenderer.FromImage(canvas))
                            {
                                canvasRenderer.SetBoundable(renderer.GetBoundable());
                                canvasRenderer.TranslateTransform(-bounds.X, -bounds.Y);
                                canvasRenderer.ScaleTransform(scaleX, scaleY);

                                if (Renderable)
                                    RenderInternal(canvasRenderer, RenderFillAndStroke);
                                else
                                    RenderChildren(canvasRenderer);
                            }
                            var srcRect = new RectangleF(0f, 0f, bounds.Width * scaleX, bounds.Height * scaleY);
                            if (Renderable)
                                renderer.DrawImage(canvas, bounds, srcRect, GraphicsUnit.Pixel, opacity);
                            else
                                RenderInternal(renderer, r => r.DrawImage(canvas, bounds, srcRect, GraphicsUnit.Pixel, opacity));
                        }
                    }
                }
            }
        }

        private void RenderInternal(ISvgRenderer renderer, Action<ISvgRenderer> renderMethod)
        {
            try
            {
                if (PushTransforms(renderer))
                {
                    SetClip(renderer);

                    var isMaskSet = SetMask(renderer);
                    renderMethod.Invoke(renderer);

                    if (isMaskSet)
                    {
                        ResetMask(renderer);
                    }

                    ResetClip(renderer);
                }
            }
            finally
            {
                PopTransforms(renderer);
            }
        }

        private bool RenderFilter(ISvgRenderer renderer)
        {
            var rendered = false;

            var filterPath = Filter.ReplaceWithNullIfNone();
            if (filterPath != null)
            {
                var element = OwnerDocument.IdManager.GetElementById(filterPath);
                if (element is SvgFilter)
                {
                    try
                    {
                        ((SvgFilter)element).ApplyFilter(this, renderer, r => RenderInternal(r, false));
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.ToString());
                    }
                    rendered = true;
                }
            }

            return rendered;
        }

        protected internal virtual void RenderFillAndStroke(ISvgRenderer renderer)
        {
            // If this element needs smoothing enabled turn anti-aliasing on
            if (RequiresSmoothRendering)
                renderer.SmoothingMode = SmoothingMode.AntiAlias;

            RenderFill(renderer);
            RenderStroke(renderer);

            // Reset the smoothing mode
            if (RequiresSmoothRendering && renderer.SmoothingMode == SmoothingMode.AntiAlias)
                renderer.SmoothingMode = SmoothingMode.Default;
        }

        /// <summary>
        /// Renders the fill of the <see cref="SvgVisualElement"/> to the specified <see cref="ISvgRenderer"/>
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
        protected internal virtual void RenderFill(ISvgRenderer renderer)
        {
            if (Fill != null)
                using (var brush = Fill.GetBrush(this, renderer, FixOpacityValue(FillOpacity)))
                {
                    if (brush != null)
                    {
                        var path = Path(renderer);
                        path.FillMode = FillRule == SvgFillRule.NonZero ? FillMode.Winding : FillMode.Alternate;
                        renderer.FillPath(brush, path);
                    }
                }
        }

        /// <summary>
        /// Renders the stroke of the <see cref="SvgVisualElement"/> to the specified <see cref="ISvgRenderer"/>
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
        protected internal virtual bool RenderStroke(ISvgRenderer renderer)
        {
            if (Stroke != null && Stroke != SvgPaintServer.None && StrokeWidth > 0f)
            {
                var strokeWidth = StrokeWidth.ToDeviceValue(renderer, UnitRenderingType.Other, this);
                using (var brush = Stroke.GetBrush(this, renderer, FixOpacityValue(StrokeOpacity), true))
                {
                    if (brush != null)
                    {
                        var path = Path(renderer);
                        var bounds = path.GetBounds();
                        if (path.PointCount < 1) return false;
                        if (bounds.Width <= 0f && bounds.Height <= 0f)
                        {
                            switch (StrokeLineCap)
                            {
                                case SvgStrokeLineCap.Round:
                                    using (var capPath = new GraphicsPath())
                                    {
                                        capPath.AddEllipse(path.PathPoints[0].X - strokeWidth / 2f, path.PathPoints[0].Y - strokeWidth / 2f, strokeWidth, strokeWidth);
                                        renderer.FillPath(brush, capPath);
                                    }
                                    break;
                                case SvgStrokeLineCap.Square:
                                    using (var capPath = new GraphicsPath())
                                    {
                                        capPath.AddRectangle(new RectangleF(path.PathPoints[0].X - strokeWidth / 2f, path.PathPoints[0].Y - strokeWidth / 2f, strokeWidth, strokeWidth));
                                        renderer.FillPath(brush, capPath);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(brush, strokeWidth))
                            {
                                if (StrokeDashArray != null && StrokeDashArray.Count > 0)
                                {
                                    var strokeDashArray = StrokeDashArray;
                                    if (strokeDashArray.Count % 2 != 0)
                                    {
                                        // handle odd dash arrays by repeating them once
                                        strokeDashArray = (SvgUnitCollection)StrokeDashArray.Clone();
                                        strokeDashArray.AddRange(StrokeDashArray);
                                    }
                                    var dashOffset = StrokeDashOffset;

                                    strokeWidth = Math.Max(strokeWidth, 1f);

                                    /* divide by stroke width - GDI uses stroke width as unit.*/
                                    var dashPattern = strokeDashArray.Select(u => ((u.ToDeviceValue(renderer, UnitRenderingType.Other, this) <= 0f) ? 1f :
                                        u.ToDeviceValue(renderer, UnitRenderingType.Other, this)) / strokeWidth).ToArray();
                                    var length = dashPattern.Length;

                                    if (StrokeLineCap == SvgStrokeLineCap.Round)
                                    {
                                        // to handle round caps, we have to adapt the dash pattern
                                        // by increasing the dash length by the stroke width - GDI draws the rounded 
                                        // edge inside the dash line, SVG draws it outside the line
                                        var pattern = new float[length];
                                        var offset = 1; // the values are already normalized to dash width
                                        for (var i = 0; i < length; i++)
                                        {
                                            pattern[i] = dashPattern[i] + offset;
                                            if (pattern[i] <= 0f)
                                            {
                                                // overlapping caps - remove the gap for simplicity, see #508
                                                if (i < length - 1)
                                                {
                                                    // add the next dash segment to the current one
                                                    dashPattern[i - 1] += dashPattern[i] + dashPattern[i + 1];
                                                    length -= 2;
                                                    for (var k = i; k < length; k++)
                                                        dashPattern[k] = dashPattern[k + 2];

                                                    // and handle the combined segment again
                                                    i -= 2;
                                                }
                                                else if (i > 2)
                                                {
                                                    // add the last dash segment to the first one
                                                    // this will change the start point, so adapt the offset
                                                    var dashLength = dashPattern[i - 1] + dashPattern[i];
                                                    pattern[0] += dashLength;
                                                    length -= 2;
                                                    dashOffset += dashLength * strokeWidth;
                                                }
                                                else
                                                {
                                                    // we have only one dash with the gap too small -
                                                    // do not use dash at all
                                                    length = 0;
                                                    break;
                                                }
                                            }
                                            offset *= -1; // increase dash length, decrease spaces
                                        }
                                        if (length > 0)
                                        {
                                            if (length < dashPattern.Length)
                                                Array.Resize(ref pattern, length);
                                            dashPattern = pattern;
                                            pen.DashCap = DashCap.Round;
                                        }
                                    }

                                    if (length > 0)
                                    {
                                        pen.DashPattern = dashPattern;

                                        if (dashOffset != 0f)
                                        {
                                            pen.DashOffset = ((dashOffset.ToDeviceValue(renderer, UnitRenderingType.Other, this) <= 0f) ? 1f :
                                                dashOffset.ToDeviceValue(renderer, UnitRenderingType.Other, this)) / strokeWidth;
                                        }
                                    }
                                }
                                switch (StrokeLineJoin)
                                {
                                    case SvgStrokeLineJoin.Bevel:
                                        pen.LineJoin = LineJoin.Bevel;
                                        break;
                                    case SvgStrokeLineJoin.Round:
                                        pen.LineJoin = LineJoin.Round;
                                        break;
                                    case SvgStrokeLineJoin.MiterClip:
                                        pen.LineJoin = LineJoin.MiterClipped;
                                        break;
                                    // System.Drawing has no support for Arcs unfortunately
                                    default:
                                        pen.LineJoin = LineJoin.Miter;
                                        break;
                                }
                                pen.MiterLimit = StrokeMiterLimit;
                                switch (StrokeLineCap)
                                {
                                    case SvgStrokeLineCap.Round:
                                        pen.StartCap = LineCap.Round;
                                        pen.EndCap = LineCap.Round;
                                        break;
                                    case SvgStrokeLineCap.Square:
                                        pen.StartCap = LineCap.Square;
                                        pen.EndCap = LineCap.Square;
                                        break;
                                }

                                renderer.DrawPath(pen, path);

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the clipping region of the specified <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to have its clipping region set.</param>
        protected internal virtual void SetClip(ISvgRenderer renderer)
        {
            var clipPath = this.ClipPath.ReplaceWithNullIfNone();
            var clip = this.Clip;
            if (clipPath != null || !string.IsNullOrEmpty(clip))
            {
                this._previousClip = renderer.GetClip();

                if (clipPath != null)
                {
                    var element = this.OwnerDocument.GetElementById<SvgClipPath>(clipPath.ToString());
                    if (element != null)
                        renderer.SetClip(element.GetClipRegion(this, renderer), CombineMode.Intersect);
                }

                if (!string.IsNullOrEmpty(clip) && clip.StartsWith("rect("))
                {
                    clip = clip.Trim();
                    var offsets = (from o in clip.Substring(5, clip.Length - 6).Split(',')
                                   select float.Parse(o.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture)).ToList();
                    var bounds = this.Bounds;
                    var clipRect = new RectangleF(bounds.Left + offsets[3], bounds.Top + offsets[0],
                                                  bounds.Width - (offsets[3] + offsets[1]),
                                                  bounds.Height - (offsets[2] + offsets[0]));
                    renderer.SetClip(new Region(clipRect), CombineMode.Intersect);
                }
            }
        }

        /// <summary>
        /// Resets the clipping region of the specified <see cref="ISvgRenderer"/> back to where it was before the <see cref="SetClip"/> method was called.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to have its clipping region reset.</param>
        protected internal virtual void ResetClip(ISvgRenderer renderer)
        {
            if (this._previousClip != null)
            {
                renderer.SetClip(this._previousClip);
                this._previousClip = null;
            }
        }

        protected internal virtual bool SetMask(ISvgRenderer renderer)
        {
            var maskPath = this.Mask.ReplaceWithNullIfNone();

            if (maskPath != null)
            {
                this._previousMask = renderer.GetMask();

                var element = this.OwnerDocument.GetElementById<SvgMask>(maskPath.ToString());
                if (element != null)
                {
                    renderer.SetMask(element.RenderMask(renderer));

                    return true;
                }
            }

            return false;
        }

        protected internal virtual void ResetMask(ISvgRenderer renderer)
        {
            renderer.DisposeMask();
            renderer.SetMask(this._previousMask);
        }

        /// <summary>
        /// Sets the clipping region of the specified <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to have its clipping region set.</param>
        void ISvgClipable.SetClip(ISvgRenderer renderer)
        {
            this.SetClip(renderer);
        }

        /// <summary>
        /// Resets the clipping region of the specified <see cref="ISvgRenderer"/> back to where it was before the <see cref="SetClip"/> method was called.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to have its clipping region reset.</param>
        void ISvgClipable.ResetClip(ISvgRenderer renderer)
        {
            this.ResetClip(renderer);
        }
    }
}
