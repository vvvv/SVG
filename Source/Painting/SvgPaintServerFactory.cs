using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Svg
{
    public class SvgPaintServerFactory : TypeConverter
    {
        public static SvgPaintServer Parse(ReadOnlySpan<char> value)
        {
            if (value.Length == 0)
            {
                return SvgPaintServer.NotSet;
            }

            var colorValue = value.Trim();

            // If it's pointing to a paint server

            if (colorValue.Length == 0)
            {
                return SvgPaintServer.NotSet;
            }

            if (colorValue.CompareTo("none".AsSpan(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                return SvgPaintServer.None;
            }

            if (colorValue.CompareTo("currentColor".AsSpan(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new SvgDeferredPaintServer("currentColor");
            }

            if (colorValue.CompareTo("inherit".AsSpan(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                return SvgPaintServer.Inherit;
            }

            if (colorValue.StartsWith("url(".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                var nextIndex = colorValue.IndexOf(')') + 1;
                var id = colorValue.Slice(0, nextIndex);

                colorValue = colorValue.Slice(nextIndex).Trim();
                var fallbackServer = colorValue.Length == 0 ? null : Parse(colorValue);

                return new SvgDeferredPaintServer(id.ToString(), fallbackServer);
            }

            // Otherwise try and parse as colour

            var color = SvgColourConverter.Parse(colorValue);
            return color == Color.Empty ? SvgPaintServer.NotSet : new SvgColourServer(color);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                return Parse(s.AsSpan());
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                // check for constant
                if (value == SvgPaintServer.None || value == SvgPaintServer.Inherit || value == SvgPaintServer.NotSet)
                    return value.ToString();

                var colourServer = value as SvgColourServer;
                if (colourServer != null)
                {
                    return new SvgColourConverter().ConvertTo(colourServer.Colour, typeof(string));
                }

                var deferred = value as SvgDeferredPaintServer;
                if (deferred != null)
                {
                    return deferred.ToString();
                }

                if (value != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "url(#{0})", ((SvgPaintServer)value).ID);
                }
                else
                {
                    return "none";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
