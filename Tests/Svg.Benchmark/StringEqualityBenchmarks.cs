using System;
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class StringEqualityBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void StringEqualityOperator()
        {
            var colour = "ActiveBorder".Trim().ToLowerInvariant();

            var equal = colour == "activeborder";
        }

        [Benchmark]
        public bool MemoryExtensionsSequenceEqual()
        {
            var colour = "ActiveBorder".AsSpan().Trim();

            Span<char> buffer = stackalloc char[32];
            var length = colour.ToLowerInvariant(buffer);

            var span = buffer.Slice(0, length);

            return span.SequenceEqual("activeborder".AsSpan());
        }

        [Benchmark]
        public bool MemoryExtensionsSequenceCompareTo()
        {
            var colour = "ActiveBorder".AsSpan().Trim();

            Span<char> buffer = stackalloc char[32];
            var length = colour.ToLowerInvariant(buffer);

            var span = buffer.Slice(0, length);

            return span.SequenceCompareTo("activeborder".AsSpan()) == 0;
        }

        [Benchmark]
        public bool MemoryExtensionsCompareToOrdinal()
        {
            var colour = "ActiveBorder".AsSpan().Trim();

            Span<char> buffer = stackalloc char[32];
            var length = colour.ToLowerInvariant(buffer);

            var span = buffer.Slice(0, length);

            return MemoryExtensions.CompareTo(span, "activeborder".AsSpan(), StringComparison.Ordinal) == 0;
        }

        [Benchmark]
        public bool MemoryExtensionsCompareToInvariantCultureIgnoreCase()
        {
            var colour = "ActiveBorder".AsSpan().Trim();
            return MemoryExtensions.CompareTo(colour, "activeborder".AsSpan(), StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        [Benchmark]
        public bool MemoryExtensionsCompareToOrdinalIgnoreCase()
        {
            var colour = "ActiveBorder".AsSpan().Trim();
            return MemoryExtensions.CompareTo(colour, "activeborder".AsSpan(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        [Benchmark]
        public bool SpanManualEquality()
        {
            var colour = "ActiveBorder".AsSpan().Trim();

            Span<char> buffer = stackalloc char[32];
            var length = colour.ToLowerInvariant(buffer);

            var span = buffer.Slice(0, length);

            if (span.Length != colour.Length)
            {
                return false;
            }

            var other = "activeborder".AsSpan();
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] != other[i])
                    return false;
            }

            return true;
        }
    }
}