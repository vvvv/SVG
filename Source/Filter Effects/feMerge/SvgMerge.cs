using System.Drawing;
using System.Linq;

namespace Svg.FilterEffects
{
    [SvgElement("feMerge")]
    public partial class SvgMerge : SvgFilterPrimitive
    {
        public override void Process(ImageBuffer buffer)
        {
            if (!this.HasChildren())
            {
                return;
            }
            var children = this.Children.OfType<SvgMergeNode>().ToList();
            var inputImage = buffer[children.First().Input];
            var result = new Bitmap(inputImage.Width, inputImage.Height);
            using (var g = Graphics.FromImage(result))
            {
                foreach (var child in children)
                {
                    g.DrawImage(buffer[child.Input], new Rectangle(0, 0, inputImage.Width, inputImage.Height),
                                0, 0, inputImage.Width, inputImage.Height, GraphicsUnit.Pixel);
                }
                g.Flush();
            }
            buffer[this.Result] = result;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgMerge>();
        }
    }
}
