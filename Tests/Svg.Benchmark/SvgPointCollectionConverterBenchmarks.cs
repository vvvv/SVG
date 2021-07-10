using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using Svg;

namespace Svg.Benchmark
{
    public class SvgPointCollectionConverterBenchmarks
    {
        [Benchmark]
        public void SvgPointCollectionConverter_Parse()
        {
            SvgPointCollectionConverter.Parse("1.6,3.2 1.2,5".AsSpan());
        }
    }
}
