using System.ComponentModel;

namespace Svg.FilterEffects
{
    [SvgElement("feDiffuseLighting")]
    public partial class SvgDiffuseLighting : SvgFilterPrimitive
    {
        [SvgAttribute("surfaceScale")]
        public float SurfaceScale
        {
            get { return GetAttribute("surfaceScale", false, 1f); }
            set { Attributes["surfaceScale"] = value; }
        }

        [SvgAttribute("diffuseConstant")]
        public float DiffuseConstant
        {
            get { return GetAttribute("diffuseConstant", false, 1f); }
            set { Attributes["diffuseConstant"] = value; }
        }

        [SvgAttribute("kernelUnitLength")]
        public SvgNumberCollection KernelUnitLength
        {
            get { return GetAttribute("kernelUnitLength", false, new SvgNumberCollection() { 1f, 1f }); }
            set { Attributes["kernelUnitLength"] = value; }
        }

        [SvgAttribute("lighting-color")]
        [TypeConverter(typeof(SvgPaintServerFactory))]
        public SvgPaintServer LightingColor
        {
            get { return GetAttribute<SvgPaintServer>("lighting-color", true, new SvgColourServer(System.Drawing.Color.White)); }
            set { Attributes["lighting-color"] = value; }
        }

        public SvgElement LightSource
        {
            get
            {
                if (this.HasChildren())
                {
                    foreach (var child in this.Children)
                    {
                        if (child is SvgDistantLight || child is SvgPointLight || child is SvgSpotLight)
                            return child;
                    }
                }
                return null;
            }
        }

        public override void Process(ImageBuffer buffer)
        {
            // TODO: Implement feDiffuseLighting filter Process().
            buffer[Result] = buffer[Input];
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgDiffuseLighting>();
        }
    }
}
