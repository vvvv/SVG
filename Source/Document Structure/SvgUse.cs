using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("use")]
    public partial class SvgUse : SvgVisualElement
    {
        [SvgAttribute("href", SvgAttributeAttribute.XLinkNamespace)]
        public virtual Uri ReferencedElement
        {
            get { return GetAttribute<Uri>("href", false); }
            set { Attributes["href"] = value; }
        }

        private bool ElementReferencesUri(SvgElement element, List<Uri> elementUris)
        {
            var useElement = element as SvgUse;
            if (useElement != null)
            {
                if (elementUris.Contains(useElement.ReferencedElement))
                {
                    return true;
                }
                // also detect cycles in referenced elements
                var refElement = this.OwnerDocument.IdManager.GetElementById(useElement.ReferencedElement);
                if (refElement is SvgUse)
                {
                    elementUris.Add(useElement.ReferencedElement);
                }
                return useElement.ReferencedElementReferencesUri(elementUris);
            }
            var groupElement = element as SvgGroup;
            if (groupElement != null && groupElement.HasChildren())
            {
                foreach (var child in groupElement.Children)
                {
                    if (ElementReferencesUri(child, elementUris))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ReferencedElementReferencesUri(List<Uri> elementUris)
        {
            var refElement = this.OwnerDocument.IdManager.GetElementById(ReferencedElement);
            return ElementReferencesUri(refElement, elementUris);
        }

        /// <summary>
        /// Checks for any direct or indirect recursions in referenced elements,
        /// including recursions via groups.
        /// </summary>
        /// <returns>True if any recursions are found.</returns>
        private bool HasRecursiveReference()
        {
            var refElement = this.OwnerDocument.IdManager.GetElementById(ReferencedElement);
            var uris = new List<Uri>() { ReferencedElement };
            return ElementReferencesUri(refElement, uris);
        }

        [SvgAttribute("x")]
        public virtual SvgUnit X
        {
            get { return GetAttribute<SvgUnit>("x", false, 0f); }
            set { Attributes["x"] = value; }
        }

        [SvgAttribute("y")]
        public virtual SvgUnit Y
        {
            get { return GetAttribute<SvgUnit>("y", false, 0f); }
            set { Attributes["y"] = value; }
        }

        [SvgAttribute("width")]
        public virtual SvgUnit Width
        {
            get { return GetAttribute<SvgUnit>("width", false, 0f); }
            set { Attributes["width"] = value; }
        }

        [SvgAttribute("height")]
        public virtual SvgUnit Height
        {
            get { return GetAttribute<SvgUnit>("height", false, 0f); }
            set { Attributes["height"] = value; }
        }

        /// <summary>
        /// Applies the required transforms to <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
        protected internal override bool PushTransforms(ISvgRenderer renderer)
        {
            if (!base.PushTransforms(renderer))
                return false;
            renderer.TranslateTransform(X.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this),
                                        Y.ToDeviceValue(renderer, UnitRenderingType.Vertical, this),
                                        MatrixOrder.Prepend);
            return true;
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            SvgVisualElement element = (SvgVisualElement)this.OwnerDocument.IdManager.GetElementById(this.ReferencedElement);
            return (element != null && !this.HasRecursiveReference()) ? element.Path(renderer) : null;
        }

        /// <summary>
        /// Gets an <see cref="SvgPoint"/> representing the top left point of the rectangle.
        /// </summary>
        public SvgPoint Location
        {
            get { return new SvgPoint(X, Y); }
        }

        /// <summary>
        /// Gets the bounds of the element.
        /// </summary>
        /// <value>The bounds.</value>
        public override RectangleF Bounds
        {
            get
            {
                var ew = this.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, this);
                var eh = this.Height.ToDeviceValue(null, UnitRenderingType.Vertical, this);
                if (ew > 0 && eh > 0)
                    return TransformedBounds(new RectangleF(this.Location.ToDeviceValue(null, this),
                        new SizeF(ew, eh)));
                var element = this.OwnerDocument.IdManager.GetElementById(this.ReferencedElement) as SvgVisualElement;
                if (element != null)
                {
                    return element.Bounds;
                }

                return new RectangleF();
            }
        }

        protected override bool Renderable { get { return false; } }

        protected override void RenderChildren(ISvgRenderer renderer)
        {
            if (ReferencedElement != null && !HasRecursiveReference())
            {
                var element = OwnerDocument.IdManager.GetElementById(ReferencedElement) as SvgVisualElement;
                if (element != null)
                {
                    var ew = Width.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this);
                    var eh = Height.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
                    if (ew > 0 && eh > 0)
                    {
                        var viewBox = element.Attributes.GetAttribute<SvgViewBox>("viewBox");
                        if (viewBox != SvgViewBox.Empty && Math.Abs(ew - viewBox.Width) > float.Epsilon && Math.Abs(eh - viewBox.Height) > float.Epsilon)
                        {
                            var sw = ew / viewBox.Width;
                            var sh = eh / viewBox.Height;
                            renderer.ScaleTransform(sw, sh, MatrixOrder.Prepend);
                        }
                    }

                    var origParent = element.Parent;
                    element._parent = this;
                    // as the new parent may have other styles that are inherited,
                    // we have to redraw the paths for the children
                    element.InvalidateChildPaths();
                    element.RenderElement(renderer);
                    element._parent = origParent;
                }
            }
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgUse>();
        }
    }
}
