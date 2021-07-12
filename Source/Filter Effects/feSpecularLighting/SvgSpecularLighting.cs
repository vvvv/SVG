using System.ComponentModel;

namespace Svg.FilterEffects
{
    [SvgElement("feSpecularLighting")]
    public partial class SvgSpecularLighting : SvgFilterPrimitive
    {
        [SvgAttribute("surfaceScale")]
        public float SurfaceScale
        {
            get { return GetAttribute("surfaceScale", false, 1f); }
            set { Attributes["surfaceScale"] = value; }
        }

        [SvgAttribute("specularConstant")]
        public float SpecularConstant
        {
            get { return GetAttribute("specularConstant", false, 1f); }
            set { Attributes["specularConstant"] = value; }
        }

        [SvgAttribute("specularExponent")]
        public float SpecularExponent
        {
            get { return GetAttribute("specularExponent", false, 1f); }
            set { Attributes["specularExponent"] = value; }
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
            // TODO: Implement feSpecularLighting filter Process().
            buffer[Result] = buffer[Input];
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgSpecularLighting>();
        }
    }
}
