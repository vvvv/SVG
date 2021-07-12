using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    /// <summary>
    /// The 'foreignObject' element allows for inclusion of a foreign namespace which has its graphical content drawn by a different user agent
    /// </summary>
    [SvgElement("foreignObject")]
    public partial class SvgForeignObject : SvgVisualElement
    {
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

        ///// <summary>
        ///// Renders the <see cref="SvgElement"/> and contents to the specified <see cref="Graphics"/> object.
        ///// </summary>
        ///// <param name="renderer">The <see cref="Graphics"/> object to render to.</param>
        //protected override void Render(SvgRenderer renderer)
        //{
        //    if (!Visible || !Displayable)
        //        return;

        //    this.PushTransforms(renderer);
        //    this.SetClip(renderer);
        //    base.RenderChildren(renderer);
        //    this.ResetClip(renderer);
        //    this.PopTransforms(renderer);
        //}

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgForeignObject>();
        }
    }
}
