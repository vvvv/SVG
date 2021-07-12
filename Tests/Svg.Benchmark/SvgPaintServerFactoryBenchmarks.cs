using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class SvgPaintServerFactoryBenchmarks
    {
        [Benchmark]
        public void SvgPaintServerFactory_Parse_none()
        {
            SvgPaintServerFactory.Parse("none".AsSpan());
        }

        [Benchmark]
        public void SvgPaintServerFactory_Parse_currentColor()
        {
            SvgPaintServerFactory.Parse("currentColor".AsSpan());
        }

        [Benchmark]
        public void SvgPaintServerFactory_Parse_inherit()
        {
            SvgPaintServerFactory.Parse("inherit".AsSpan());
        }

        [Benchmark]
        public void SvgPaintServerFactory_Parse_url()
        {
            SvgPaintServerFactory.Parse("url(#Grad1)".AsSpan());
        }

        [Benchmark]
        public void SvgPaintServerFactory_Parse_url_with_fallback()
        {
            SvgPaintServerFactory.Parse("url(#invisible1) lime".AsSpan());
        }
    }
}
