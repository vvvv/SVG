using System;
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class SvgNumberCollectionConverterBenchmarks
    {
        [Benchmark]
        public void SvgNumberCollectionConverter_Parse()
        {
            SvgNumberCollectionConverter.Parse("1.6 3.2 1.2 5".AsSpan());
            SvgNumberCollectionConverter.Parse("1.6,3.2,1.2,5".AsSpan());
        }
    }
}
