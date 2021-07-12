using System;
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class ToLowerInvariantBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void ToLowerInvariantString()
        {
            var colour = "ActiveBorder".Trim().ToLowerInvariant();
        }

        [Benchmark]
        public void ToLowerInvariantSpan()
        {
            var colour = "ActiveBorder".AsSpan().Trim();
            Span<char> buffer = stackalloc char[colour.Length];
            colour.ToLowerInvariant(buffer);
        }

        [Benchmark]
        public void ToLowerInvariantSpanCustom()
        {
            var colour = "ActiveBorder".AsSpan().Trim();
            
            Span<char> buffer = stackalloc char[colour.Length];

            for (int i = 0; i < colour.Length; i++)
            {
                buffer[i] = char.ToLowerInvariant(colour[i]);
            }
        }

        [Benchmark]
        public void ToLowerInvariantSpanCustomFromString()
        {
            var colour = "ActiveBorder";
 
            Span<char> buffer = stackalloc char[colour.Length];

            for (int i = 0; i < colour.Length; i++)
            {
                buffer[i] = char.ToLowerInvariant(colour[i]);
            }
        }

        [Benchmark]
        public void ToLowerAsciiSpanCustomFromString()
        {
            var colour = "ActiveBorder";
            Span<char> buffer = stackalloc char[colour.Length];
            ColorConverterHelpers.ToLowerAscii(colour.AsSpan(), ref buffer);
            var trimmed = ((ReadOnlySpan<char>)buffer).Trim();
        }

        [Benchmark]
        public void ToLowerAsciiSpanCustomFromStringTrimmed()
        {
            var colour = "ActiveBorder".AsSpan().Trim();
            Span<char> buffer = stackalloc char[colour.Length];
            ColorConverterHelpers.ToLowerAscii(colour, ref buffer);
        }
    }
}