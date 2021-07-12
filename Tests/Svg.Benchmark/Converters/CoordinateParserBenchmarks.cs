using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using Svg;

namespace Svg.Benchmark
{
    public class CoordinateParserBenchmarks
    {
        [Benchmark]
        public void CoordinateParser_TryGetBool()
        {
            var chars = "false".AsSpan().Trim();
            var state = new CoordinateParserState(ref chars);
            CoordinateParser.TryGetBool(out var result, ref chars, ref state);
        }

        [Benchmark]
        public void CoordinateParser_TryGetFloat_Points()
        {
            var chars = "1.6,3.2 1.2,5".AsSpan().Trim();
            var state = new CoordinateParserState(ref chars);
            while (CoordinateParser.TryGetFloat(out var result, ref chars, ref state))
            {
            }
        }
    }
}
