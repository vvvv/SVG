using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Svg.Helpers;

namespace Svg
{
    /// <summary>
    /// Represents a list of <see cref="SvgUnit"/> used with the <see cref="SvgPolyline"/> and <see cref="SvgPolygon"/>.
    /// </summary>
    [TypeConverter(typeof(SvgPointCollectionConverter))]
    public class SvgPointCollection : List<SvgUnit>, ICloneable
    {
        public object Clone()
        {
            var points = new SvgPointCollection();
            foreach (var point in this)
                points.Add(point);
            return points;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < Count; i += 2)
            {
                if (i + 1 < Count)
                {
                    if (i > 1)
                    {
                        builder.Append(" ");
                    }
                    // we don't need unit type
                    builder.Append(this[i].Value.ToSvgString());
                    builder.Append(",");
                    builder.Append(this[i + 1].Value.ToSvgString());
                }
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// A class to convert string into <see cref="SvgPointCollection"/> instances.
    /// </summary>
    public class SvgPointCollectionConverter : TypeConverter
    {
        private static readonly char[] SplitChars = new[] { ' ', '\t', '\n', '\r', ',' };

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                return Parse(s.AsSpan());
            }

            return base.ConvertFrom(context, culture, value);
        }

        public static SvgPointCollection Parse(ReadOnlySpan<char> points)
        {
#if false
            var coords = points.Trim();
            var state = new CoordinateParserState(ref coords);
            var result = new SvgPointCollection();
            while (CoordinateParser.TryGetFloat(out var pointValue, ref coords, ref state))
            {
                result.Add(new SvgUnit(SvgUnitType.User, pointValue));
            }
            return result;
#else
            var collection = new SvgPointCollection();
            var splitChars = SplitChars.AsSpan();
            var parts = new StringSplitEnumerator(points, splitChars);

            foreach (var part in parts)
            {
                var partValue = part.Value;
                var result = StringParser.ToFloat(ref partValue);
                collection.Add(new SvgUnit(SvgUnitType.User, result));
            }

            return collection;
#endif
        }
    }
}
