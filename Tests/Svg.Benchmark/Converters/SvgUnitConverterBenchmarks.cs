using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using Svg;
using Svg.Transforms;

namespace Svg.Benchmark
{
    public class SvgUnitConverterBenchmarks
    {
        [Benchmark]
        public void SvgUnitConverter_Parse()
        {
            SvgUnitConverter.Parse("1pt".AsSpan());
            SvgUnitConverter.Parse("1.25px".AsSpan());
            SvgUnitConverter.Parse("1pc".AsSpan());
            SvgUnitConverter.Parse("15px".AsSpan());
            SvgUnitConverter.Parse("1mm".AsSpan());
            SvgUnitConverter.Parse("3.543307px".AsSpan());
            SvgUnitConverter.Parse("1cm".AsSpan());
            SvgUnitConverter.Parse("35.43307px".AsSpan());
            SvgUnitConverter.Parse("1in".AsSpan());
            SvgUnitConverter.Parse("90px".AsSpan());
            SvgUnitConverter.Parse("15em".AsSpan());
            SvgUnitConverter.Parse("0.2822222mm".AsSpan());
            SvgUnitConverter.Parse("3990".AsSpan());
            SvgUnitConverter.Parse("1990".AsSpan());
            SvgUnitConverter.Parse("-50".AsSpan());
            SvgUnitConverter.Parse(".4in".AsSpan());
            SvgUnitConverter.Parse(".25em".AsSpan());
            SvgUnitConverter.Parse("10%".AsSpan());
            SvgUnitConverter.Parse("1%".AsSpan());
            SvgUnitConverter.Parse("0%".AsSpan());
            SvgUnitConverter.Parse("100%".AsSpan());
            SvgUnitConverter.Parse("1.2em".AsSpan());
            SvgUnitConverter.Parse("medium".AsSpan());
            SvgUnitConverter.Parse("x-small".AsSpan());
            SvgUnitConverter.Parse("xx-large".AsSpan());
            SvgUnitConverter.Parse("657.45".AsSpan());
            SvgUnitConverter.Parse("12.5".AsSpan());
            SvgUnitConverter.Parse("0".AsSpan());
            SvgUnitConverter.Parse("12".AsSpan());
        }
    }
}
