using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    /// <summary>
    /// Defines a path that can be used by other <see cref="ISvgClipable"/> elements.
    /// </summary>
    [SvgElement("clipPath")]
    public partial class SvgClipPath : SvgElement
    {
        private GraphicsPath _path;

        /// <summary>
        /// Specifies the coordinate system for the clipping path.
        /// </summary>
        [SvgAttribute("clipPathUnits")]
        public SvgCoordinateUnits ClipPathUnits
        {
            get { return GetAttribute("clipPathUnits", false, SvgCoordinateUnits.UserSpaceOnUse); }
            set { Attributes["clipPathUnits"] = value; }
        }

        /// <summary>
        /// Gets this <see cref="SvgClipPath"/>'s region to be used as a clipping region.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="renderer"></param>
        /// <returns>A new <see cref="Region"/> containing the <see cref="Region"/> to be used for clipping.</returns>
        public Region GetClipRegion(SvgVisualElement owner, ISvgRenderer renderer)
        {
            if (_path == null || IsPathDirty)
            {
                _path = new GraphicsPath();

                if (HasChildren())
                {
                    foreach (var element in Children)
                        CombinePaths(_path, element, renderer);
                }

                IsPathDirty = false;
            }

            var result = _path;
            if (ClipPathUnits == SvgCoordinateUnits.ObjectBoundingBox)
            {
                result = (GraphicsPath)_path.Clone();
                using (var transform = new Matrix())
                {
                    var bounds = owner.Bounds;
                    transform.Scale(bounds.Width, bounds.Height, MatrixOrder.Append);
                    transform.Translate(bounds.Left, bounds.Top, MatrixOrder.Append);
                    result.Transform(transform);
                }
            }

            return new Region(result);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="element"></param>
        /// <param name="renderer"></param>
        private void CombinePaths(GraphicsPath path, SvgElement element, ISvgRenderer renderer)
        {
            var graphicsElement = element as SvgVisualElement;
            if (graphicsElement != null)
            {
                var childPath = graphicsElement.Path(renderer);
                if (childPath != null)
                {
                    path.FillMode = graphicsElement.ClipRule == SvgClipRule.NonZero ? FillMode.Winding : FillMode.Alternate;

                    if (graphicsElement.Transforms != null)
                        using (var matrix = graphicsElement.Transforms.GetMatrix())
                            childPath.Transform(matrix);

                    if (childPath.PointCount > 0)
                        path.AddPath(childPath, false);
                }
            }

            if (HasChildren())
            {
                foreach (var child in element.Children)
                    CombinePaths(path, child, renderer);
            }
        }

        /// <summary>
        /// Called by the underlying <see cref="SvgElement"/> when an element has been added to the
        /// 'Children' collection.
        /// </summary>
        /// <param name="child">The <see cref="SvgElement"/> that has been added.</param>
        /// <param name="index">An <see cref="int"/> representing the index where the element was added to the collection.</param>
        protected override void AddElement(SvgElement child, int index)
        {
            base.AddElement(child, index);
            IsPathDirty = true;
        }

        /// <summary>
        /// Called by the underlying <see cref="SvgElement"/> when an element has been removed from the
        /// <see cref="SvgElement.Children"/> collection.
        /// </summary>
        /// <param name="child">The <see cref="SvgElement"/> that has been removed.</param>
        protected override void RemoveElement(SvgElement child)
        {
            base.RemoveElement(child);
            IsPathDirty = true;
        }

        /// <summary>
        /// Renders the <see cref="SvgElement"/> and contents to the specified <see cref="ISvgRenderer"/> object.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
        protected override void Render(ISvgRenderer renderer)
        {
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgClipPath>();
        }
    }
}
