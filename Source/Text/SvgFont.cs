using System.Linq;

namespace Svg
{
    [SvgElement("font")]
    public partial class SvgFont : SvgElement
    {
        [SvgAttribute("horiz-adv-x")]
        public float HorizAdvX
        {
            get { return GetAttribute("horiz-adv-x", true, 0f); }
            set { Attributes["horiz-adv-x"] = value; }
        }

        [SvgAttribute("horiz-origin-x")]
        public float HorizOriginX
        {
            get { return GetAttribute("horiz-origin-x", true, 0f); }
            set { Attributes["horiz-origin-x"] = value; }
        }

        [SvgAttribute("horiz-origin-y")]
        public float HorizOriginY
        {
            get { return GetAttribute("horiz-origin-y", true, 0f); }
            set { Attributes["horiz-origin-y"] = value; }
        }

        [SvgAttribute("vert-adv-y")]
        public float VertAdvY
        {
            get { return GetAttribute("vert-adv-y", true, HasChildren() ? Children.OfType<SvgFontFace>().First().UnitsPerEm : 0f); }
            set { Attributes["vert-adv-y"] = value; }
        }

        [SvgAttribute("vert-origin-x")]
        public float VertOriginX
        {
            get { return GetAttribute("vert-origin-x", true, HorizAdvX / 2); }
            set { Attributes["vert-origin-x"] = value; }
        }

        [SvgAttribute("vert-origin-y")]
        public float VertOriginY
        {
            get
            {
                var defaultValue = this.HasChildren() ? Children.OfType<SvgFontFace>().First().Attributes["ascent"] as float? ?? 0f : 0f;
                return GetAttribute("vert-origin-y", true, defaultValue);
            }
            set { Attributes["vert-origin-y"] = value; }
        }

        public override SvgElement DeepCopy()
        {
            return base.DeepCopy<SvgFont>();
        }

        protected override void Render(ISvgRenderer renderer)
        {
        }
    }
}
